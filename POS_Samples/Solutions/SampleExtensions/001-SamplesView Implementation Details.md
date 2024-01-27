# Samples View Implementation details:
- Firstly define an interface:
  ```ts
  export interface ISampleItem {
    label: string;
    viewName?: string;
    items?: ISampleItem[];
}
  ```



