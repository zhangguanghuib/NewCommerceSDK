# How to consume jQuery in Commerce SDK  POS extension project
Please find the official doc for more details:

https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/pos-extension/knockout-pos-extension, especially this section:
<img width="632" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5294225b-6cc2-4ca4-bc42-c6ccf787a678">

### Step 1.   Download this Nuget Package:
https://www.nuget.org/packages/jquery.TypeScript.DefinitelyTyped/<br/>
<img width="917" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d11cadb3-6deb-4915-ab84-9907a69154f1"><br/>
<img width="708" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b4561ba9-0be6-4d96-bb4c-816ac392b7e2"><br/>


### Step 2. Go to Commerce SDK  Project,  add these 3 packages:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/fbb387b2-47f3-4023-8def-d980f4a97cfb)

### Step 3. Add these three sections:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/08d367ee-ee59-465d-93b6-9ee891055e10)

In this steps, please check the below folder to make sure the PackageDefinition.Name is correctlt set:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/a022c62b-99fc-4852-a507-f5249a3be7e7)

### Step 4. Build the POS project, or the whole solution, you will see the js file will be copied to the Libraries folder 
<img width="1004" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f22d4c8e-315d-4826-93e9-248356fbd4f3">






