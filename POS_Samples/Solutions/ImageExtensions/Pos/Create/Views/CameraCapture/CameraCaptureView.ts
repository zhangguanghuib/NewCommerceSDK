import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import ko from "knockout";

export default class CameraCaptureView extends Views.CustomViewControllerBase {
    public qrText: ko.Observable<string>;
    public capturedImageBase64: ko.Observable<string>;
    constructor(context: Views.ICustomViewControllerContext, options?: any) {
        super(context);
        this.state.title = "Camera Capture View";
        this.qrText = ko.observable("");
        this.capturedImageBase64 = ko.observable("");
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public uploadImage = (data: any, event: Event): void => {
        const input: HTMLInputElement = event.target as HTMLInputElement;

        if (input.files && input.files[0]) {
            const file: File = input.files[0];
            const reader: FileReader = new FileReader();

            reader.onload = (e: ProgressEvent<FileReader>) => {
                const base64Image = e.target?.result as string;
                this.capturedImageBase64(base64Image);
                this.saveImageToDatabase(base64Image);
            };

            reader.readAsDataURL(file);
        } else {
            alert("No image selected!");
        }
    }

    public saveImageToDatabase = (base64Image: string) => {
        console.log("Saving image to database...", base64Image);
    };

    public captureAndSave = () => {
        document.getElementById('cameraInput')?.click();
    };
    
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}