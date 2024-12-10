#include <mysql.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <windows.h>
#include <wchar.h>

#define SERVER "127.0.0.1"
#define USER "root"
#define PASSWORD "itshardtoguess"
#define DATABASE "kanaPractice"

#pragma warning(disable : 4996)


MYSQL* connectDatabase() { // Purpose - Connect to the database (Returns a pointer to the connection)
    MYSQL* conn = mysql_init(NULL);
    if (!mysql_real_connect(conn, SERVER, USER, PASSWORD, DATABASE, 0, NULL, 0)) { 
        //fprintf(stderr, "Connection failed: %s\n", mysql_error(conn));
        exit(1);
    }
    return conn;
}

void createUser(MYSQL* conn, const char* username, const char* password) { // Purpose - Create a new user
    char query[256];
    snprintf(query, sizeof(query),
        "INSERT INTO Users (Username, PasswordHash) VALUES ('%s', '%s')",
        username, password);

    if (mysql_query(conn, query)) {
        printf("failure\n");  // Print failure for Unity to detect
        return;
    }

    // Get the new user's ID
    int userId = (int)mysql_insert_id(conn);

    // Insert default mastery rows for all Kana
    char insertMasteryQuery[512];
    snprintf(insertMasteryQuery, sizeof(insertMasteryQuery),
        "INSERT INTO UserMastery (UserID, KanaID, MasteryLevel) "
        "SELECT %d, KanaID, 0 FROM KanaCharacter", userId);

	if (mysql_query(conn, insertMasteryQuery)) { // Execute query
        printf("failure\n");
        return;
    }

    printf("success\n");  // Print success for Unity to detect
}

// Login a user
void loginUser(MYSQL* conn, const char* username, const char* password) { // Purpose - Login a user 
    char query[256];
    snprintf(query, sizeof(query),
		"SELECT Username, COALESCE(DayStreak, '0') AS DayStreak, COALESCE(LastPracticeDate, 'NULL') AS LastPracticeDate " // COALESCE to handle NULL values
        "FROM Users WHERE Username = '%s' AND PasswordHash = '%s'", username, password);

	if (mysql_query(conn, query)) { // Execute query
        printf("failure\n");
        return;
    }

	MYSQL_RES* res = mysql_store_result(conn); // Store result
    if (res == NULL) {
        printf("failure\n");
        return;
    }

    MYSQL_ROW row;
    if ((row = mysql_fetch_row(res)) != NULL) {
        // Check for missing values and default them
		const char* dayStreak = row[1] ? row[1] : "0"; // Default to 0 if NULL
		const char* lastPracticeDate = row[2] ? row[2] : "NULL"; // Default to NULL if NULL
         
        // Output result in expected format
        printf("success|%s|%s\n", dayStreak, lastPracticeDate);
    }
    else {
		printf("failure\n"); // Print failure for Unity to detect
    }

    mysql_free_result(res);
}

// Get Last Practice Date and Day Streak
void getPracticeInfo(MYSQL* conn, const char* username) { // Purpose - Get the last practice date and day streak of a user
    char query[256];
    snprintf(query, sizeof(query), "SELECT COALESCE(LastPracticeDate, 'None'), COALESCE(DayStreak, 0) FROM Users WHERE Username = '%s'", username);

    if (mysql_query(conn, query)) {
        printf("failure\n");
        return;
    }

    MYSQL_RES* res = mysql_store_result(conn);
    if (res == NULL) {
        printf("failure\n");
        return;
    }

	MYSQL_ROW row = mysql_fetch_row(res); // Fetch row
    if (row != NULL) {
		const char* lastPracticeDate = row[0] ? row[0] : "None"; // Default to None if NULL
		const char* dayStreak = row[1] ? row[1] : "0"; // Default to 0 if NULL
		printf("success;%s;%s\n", lastPracticeDate, dayStreak); // Output result in expected format
    }
    else {
        printf("failure\n");
    }

    mysql_free_result(res);
}

// Update Last Practice Date and Day Streak
void updatePracticeInfo(MYSQL* conn, const char* username, const char* lastPracticeDate, int dayStreak) { // Purpose - Update the last practice date and day streak of a user
    char query[256];
    snprintf(query, sizeof(query),
        "UPDATE Users SET LastPracticeDate = '%s', DayStreak = %d WHERE Username = '%s'", lastPracticeDate, dayStreak, username);

    if (mysql_query(conn, query)) {
        printf("failure\n");
    }
    else {
        printf("success\n");
    }
}

void getUserMastery(MYSQL* conn, int userId) {
    char query[512]; // Increased buffer size to prevent truncation
    snprintf(query, sizeof(query),
        "SELECT UserMastery.KanaID, KanaCharacter.KanaSymbol, KanaCharacter.Romaji, KanaCharacter.Type, " 
        "UserMastery.MasteryLevel, UserMastery.LastReviewed, UserMastery.NextReview "
        "FROM UserMastery "
        "JOIN KanaCharacter ON UserMastery.KanaID = KanaCharacter.KanaID "
        "WHERE UserMastery.UserID = %d AND (UserMastery.NextReview IS NULL OR UserMastery.NextReview <= CURDATE())", userId);


    if (mysql_query(conn, query)) { 
        printf("failure\n");
        return;
    }

    MYSQL_RES* res = mysql_store_result(conn);
    if (res == NULL) {
        printf("failure\n");
        return;
    }

    MYSQL_ROW row; 
    while ((row = mysql_fetch_row(res)) != NULL) {
        printf("success|%s|%s|%s|%s|%s|%s|%s\n",
			row[0], row[1], row[2], row[3], row[4], row[5] ? row[5] : "None", row[6] ? row[6] : "None"); // Output result in expected format
    }

    mysql_free_result(res);
}

void updateUserMastery(MYSQL* conn, int userId, int kanaId, float masteryLevel, const char* lastReviewed, const char* nextReview) { // Purpose - Update the mastery level of a user for a specific Kana
    char query[512];
    snprintf(query, sizeof(query),
        "UPDATE UserMastery SET MasteryLevel = %.2f, LastReviewed = '%s', NextReview = '%s' "
        "WHERE UserID = %d AND KanaID = %d", masteryLevel, lastReviewed, nextReview, userId, kanaId);

    if (mysql_query(conn, query)) {
        printf("failure\n");
    }
    else {
        printf("success\n");
    }
}

void getHighScore(MYSQL* conn, const char* username) { // Purpose - Get the high score of a user
    char query[256];
    snprintf(query, sizeof(query),
        "SELECT HighScore FROM Users WHERE Username = '%s'", username);

    if (mysql_query(conn, query)) {
        printf("failure\n");
        return;
    }

    MYSQL_RES* res = mysql_store_result(conn); 
    if (res == NULL) {
        printf("failure\n");
        return;
    }

    MYSQL_ROW row = mysql_fetch_row(res);
    if (row != NULL) {
        printf("success;%s\n", row[0]);
    }
    else {
        printf("failure\n");
    }

    mysql_free_result(res);
}

void updateHighScore(MYSQL* conn, const char* username, int highScore) { // Purpose - Update the high score of a user
    char query[256];
    snprintf(query, sizeof(query),
        "UPDATE Users SET HighScore = %d WHERE Username = '%s'",
        highScore, username);

    if (mysql_query(conn, query)) {
        printf("failure\n");
    }
    else {
        printf("success\n");
    }
}

void setConsoleToUnicode() {
    // Set the console output to UTF-16
    SetConsoleOutputCP(CP_UTF8);
	// set console mode to handle UTF-8
	SetConsoleCP(CP_UTF8);
}

// Main function to handle commands
int main(int argc, char* argv[]) {
    if (argc < 2) {
        return 1;
    }

	setConsoleToUnicode(); // Set console to UTF-8 to support Japanese characters
    MYSQL* conn = connectDatabase();

    const char* command = argv[1];
    const char* username = argv[2];
    const char* password = argv[3];

    if (strcmp(command, "login") == 0) {
        if (argc != 4) {
            printf("failure\n");
            mysql_close(conn);
            return 1;
        }
        loginUser(conn, username, password);
    }
    else if (strcmp(command, "create") == 0) {
        if (argc != 4) {
            printf("failure\n");
            mysql_close(conn);
            return 1;
        }
        createUser(conn, username, password);
	}
	else if (strcmp(command, "getPracticeInfo") == 0) {
		getPracticeInfo(conn, username);
	}
	else if (strcmp(command, "updatePracticeInfo") == 0) {
		if (argc != 5) {
			printf("failure\n");
			mysql_close(conn);
			return 1;
		}
		updatePracticeInfo(conn, username, argv[3], atoi(argv[4]));
	}
    else if (strcmp(command, "getUserMastery") == 0) {
        if (argc != 3) {
            printf("failure\n");
            mysql_close(conn);
            return 1;
        }
        getUserMastery(conn, atoi(argv[2]));
    }
	else if (strcmp(command, "updateUserMastery") == 0) {
		if (argc != 7) {
			printf("failure\n");
			mysql_close(conn);
			return 1;
		}
		updateUserMastery(conn, atoi(argv[2]), atoi(argv[3]), atof(argv[4]), argv[5], argv[6]);
	}
    else if (strcmp(command, "getHighScore") == 0) {
        getHighScore(conn, username);
    }
    else if (strcmp(command, "updateHighScore") == 0) {
        if (argc != 4) {
            printf("failure\n");
            mysql_close(conn);
            return 1;
        }
        updateHighScore(conn, username, atoi(argv[3]));
    }
    else {
        printf("failure\n");
    }

    mysql_close(conn);
    return 0;
}
// PS SPHAGHETTI CODE I KNOW! I'll clean it up later. I promise.