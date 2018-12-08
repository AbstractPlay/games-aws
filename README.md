# Abstract Play Games/AI Server

This AWS serverless stack contains the native games and AIs. It is not a public API. Only the data server can access this. See [the AP wiki page about game and AI APIs](https://www.abstractplay.com/wiki/api.thirdparty) for more details. 

## Contact

The [main website](https://www.abstractplay.com) houses the development blog and wiki.

## Change log

08 Dec 2018:

* Archiving this repository. Built-in games are moving into the API itself. The standalone game server idea has to be shelved for the time being. [See the development blog for details.](https://www.abstractplay.com)

10 Jun 2018:

* Moved to [Serverless framework](https://serverless.com) for deployment.

29 Apr 2018:

* I have Ithaka fully functioning now. This is the template I'll use for all other games. But I won't do any more games until the full user workflow is working for this one game.

20 Apr 2018:

* Initial commit of the new code. The basics are working, which is exciting.

## Deploy

* Make sure you have `dotnet` and `serverless` installed.
* Clone the repo.
* Run `npm install` to install the plugins.
* Configure `serverless` with your own credentials.
* Create the `apsecrets.yml` file with the entries you see in `serverless.yml` or enter your information directly.
* Run `serverless deploy`.
