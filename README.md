# Abstract Play Games/AI Server

This AWS serverless stack contains the native games and AIs. It is not a public API. Only the data server can access this. See [the AP wiki page about game and AI APIs](https://www.abstractplay.com/wiki/api.thirdparty) for more details. 

## Contact

The [main website](https://www.abstractplay.com) houses the development blog and wiki.

## Change log

29 Apr 2018:

* I have Ithaka fully functioning now. This is the template I'll use for all other games. But I won't do any more games until the full user workflow is working for this one game.

20 Apr 2018:

* Initial commit of the new code. The basics are working, which is exciting.

## Deploy 

This AWS-specific files are as follows:
* serverless.template - an AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* Function.cs - class file containing the C# method mapped to the single function declared in the template file
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS

### Here are some steps to follow from Visual Studio:

To deploy your Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view your deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

### Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can use the following command lines to deploy your application from the command line (these examples assume the project name is *EmptyServerless*):

Restore dependencies
```
    cd "apgames"
    dotnet restore
```

Execute unit tests
```
    cd "apgames/test/apgames.Tests"
    dotnet test
```

Deploy application
```
    cd "apgames/src/apgames"
    dotnet lambda deploy-serverless
```

