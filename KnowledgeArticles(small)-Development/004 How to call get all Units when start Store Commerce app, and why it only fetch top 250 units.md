## How to call get all Units when start Store Commerce app, and why it only fetch top 250 units?

1. From POS clients, this client API  is called:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/dfda3930-b562-4a05-989f-982850e97a21)

2. In the CSU  API, this one is called:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c668bc7a-23ba-4621-998b-27614809e11a)
   <img width="516" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9b79bc19-bc5b-4ea0-a314-5629deaa76cd">

4. Finally in the cart line, we run this code to show the POS unit:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/01ba4de5-7905-4594-b142-eb3e5f6af25c)<br/>

 5. Here is a problem: why it only fecth the top 250 units:
    <img width="539" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e3b8f279-126d-484c-a464-8f4d7665c9f2">

    That is because in product code we have a default value that is only to fetch top 250:
    <img width="747" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/1f113a45-432c-42ae-b061-c0c057046272">

  6. That will caused a problem, in case the system has over 250 units,  and the unit of the current product is not in the first 250 units,  then it will caused a problem.




