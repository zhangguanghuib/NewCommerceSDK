## How to consume external JS Libray in POS  project (Non-Nuget version)

### Step 1.  Download JS  file, and remove the js extension name
<img width="248" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/fc126622-5039-45c1-9fa0-39ac0791ccd6">

### Step 2.  Add the below section to the POS project file
```
  <Target Name="ContentIncludejsPDFLibrary" BeforeTargets="AssignTargetPaths">
    <PropertyGroup>
        <jsPDFLibraryFilePath>ExternalJSDependencies/jspdf.min</jsPDFLibraryFilePath>
        <jsPDFFile>Libraries/jspdf.min.js</jsPDFFile>
    </PropertyGroup>
    <Copy SourceFiles="$(jsPDFLibraryFilePath)" DestinationFiles="$(jsPDFFile)" SkipUnchangedFiles="false" />
    <ItemGroup>
      <Content Include="$(jsPDFFile)"></Content>
    </ItemGroup>
  </Target>

  <Target Name="ContentIncludehtml2canvasFLibrary" BeforeTargets="AssignTargetPaths">
    <PropertyGroup>
      <html2canvasLibraryFilePath>ExternalJSDependencies/html2canvas</html2canvasLibraryFilePath>
      <html2canvasFile>Libraries/html2canvas.js</html2canvasFile>
    </PropertyGroup>
    <Copy SourceFiles="$(html2canvasLibraryFilePath)" DestinationFiles="$(html2canvasFile)" SkipUnchangedFiles="false" />
    <ItemGroup>
      <Content Include="$(html2canvasFile)"></Content>
    </ItemGroup>
  </Target>
```

###  Step 3.   Build the whole solution,  you can see these JS file will be copied to Scale Unit Package
<img width="814" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/552e130e-a697-48dc-8ce4-1ddc25b80878">


###  Step 4. Then you can consume these JS Libray
```js
import jspdf from "jspdf"; 
```

1. Open POS Report:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5c537531-6bfa-4f9b-a0cb-91bd7150a474)

   Click the Command Button to export the CSV or  export to PDF

