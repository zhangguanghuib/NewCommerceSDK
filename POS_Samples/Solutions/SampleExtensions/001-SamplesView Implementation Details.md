# Samples View Implementation details:
- Firstly define an interface:
  ```ts
  export interface ISampleItem {
    label: string;
    viewName?: string;
    items?: ISampleItem[];
  }
  ```
  Please notice here viewName and items is nullable, that means some Item only has viewName, but other item only has items, then two kinds of items can resume the same type.
- Secondly define the data:<br/>
<img width="549" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/dd05152d-c4ad-4a74-9ae9-a84cf5416563"><br/>
From the data you can see:<br/>
  - The first level has label and items
  - The second level has lable and viewName
- Thirdly, in the html:<br/>
  <img width="1380" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2ca60f37-f558-4a6e-b1e6-43035c424428">
- Then finally it looks like:
  <img width="897" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/0b824c4c-24a2-4f88-a4e2-2bffb51e2cfe">






