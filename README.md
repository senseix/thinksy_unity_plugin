senseix_plugin
==============

Unity Plugin for accessing senseix resources

How to use this plugin
1. if you want to use the demo provided with plugin, please drag script trigger.cs to an object in your scene.
2. if you want to use plugin with your own project, you can call APIs provided in this plugin in your script

Quick start:
To use functions in plugin, you need to let it know the game token by senseix.initSenseix(string gameToken,int rankNum). "gameToken" is the token you got from our website for your game. "rankNum" is the size of leaderboard.
Then you can call senseix.coachLogin (string login,string password,string game) in your login dialog to let coach/parent login. "login" is the email of coach(parent) account. "password" is password for this account.
After user logins, their information will be cached, you can bundle functions senseix.coachLogout() to a button in your game to clean up user information and end current session. 
You also can call function senseix.coachSignUp (string email,string name,string password) to let user sign up a new account.

API:
int initSenseix(string gameToken)
	Developer can call this function in start up script of game and assign token of this game.
	return: -1 for fail, 0 for success.
	example: line 25 in trigger.cs

Queue pullProblemQ(int count,string category,int level)
	Pulling a set of questions from server according to the current profile/player ID, and return these questions in Queue. "count" is number of questions that are going to be pull from server. "category" is category of questions. "level" is level of questions.
	return: null for fail, a "Queue" object for success.
	example: line 215 in trigger.cs

void pushProblemA(int problem_id,int duration,bool correctness,int tries,int game_difficulty,string answer)
	Push answer and information about the answer given by player to server. "problem_id" is the ID for pushed question. "duration" is time consumed by player. "correctness" is a boolean value about whether answer is correct. "tries" is number of times that player tried. "game_difficulty" is difficulty of the game. "answer" is the content of the answer given by player.
	return: void
	example: line 308 in trigger.cs
	
bool checkAnswer(string answerStr,problem p)
bool checkAnswer(int answerInt,problem p)
	Locally check whether a answer is write.
	return: false if answer is wrong, true if answer is correct.
	example: line 308 in trigger.cs
	
int coachSignUp (string email,string name,string password)
	This function can be used to sign up a new coach/parent account. "email" is the account ID of new user. "name" is name of parent/coach. "password" is password for this account. After an account gets created by this way, a session will be started. Profile informationi will be cached in player's device.
	return: negative if failed, 0 for success.
	
int coachLogin (string login,string password)
	Start a new session. "login" is the "email" address used when this account got created. "password" is password for this account. When new session started, user information will be cached, which can be cleaned up by calling function "coachLogout".
	return: negative for fail, 0 for success.
	
int createPlayer (string playerName)
	Create a new player which belongs to current coach account. "playerName" is name for this new player.
	return: negative for fail, 0 for success.
	
Queue getPlayer ()
	Get the list of players belonging to current coach/parent account and put the into a Queue.
	return: null if fail, Queue object for success
	example: line 235 in trigger.cs to generate a list of botton that represent available players for current coach/parent account.