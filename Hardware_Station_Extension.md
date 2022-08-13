This document is to demonstrate how to develop hardware station customizatin and debug that for trouble shooting

Through this demon, you will learn how to develop these below things:
There are 3 sample provide:

* In Sample#1, you will learn: <br/>
  1. How to develop a hardware station solution.
  2. How to make a new POS page including html page, view file in Typescript,  View Mode code file in Typescript.
  3. How to open the page you just created through adding a command bar button in the existing page.
  4. How to call hardware station API  in POS TS  code.

* In Sample #2, you will learn: <br/>
  1. How to overide the existing hardware logic.

* In Sample #3, you will learn: <br/>
  1. How to delevelop a payment device.

<hr/>

<font size="4">Sample #1 -  Coin Dispenser</font>

The final effect of this sample is like this:

1. Go to show journal, click the new command button we added:
   ![image](https://user-images.githubusercontent.com/14832260/184473152-d17c3aa2-8718-465c-a965-c65342e4dba0.png)
2. The new page will open:
    ![image](https://user-images.githubusercontent.com/14832260/184473213-68e41a46-7d04-4788-8093-81ec8494f012.png)
3. Click each button to call hardware station to do health check, or dispense 10 coin, or 1000 coin,  if there is no enough coin to dispense, the hardare station will return error:
    ![image](https://user-images.githubusercontent.com/14832260/184473324-1d7a3024-0625-4aeb-aede-ea573519da60.png)


The steps of making this solution is as below:
1. Hardware station project structure are like below:
    It is .Net Standard 2.0 Class Library project:
   ![image](https://user-images.githubusercontent.com/14832260/184473410-2e0f69d1-a79e-4d27-abf3-73c8749ab881.png)

