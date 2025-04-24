import ko from "knockout";
import * as Dialogs from "PosApi/Create/Dialogs";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { IImageCaptureDialogResult } from "./IImageCaptureDialogResult";

type DialogResolve = (imageCaptureDialogResult: IImageCaptureDialogResult) => void;
type DialogReject = (reason: any) => void;

export default class ImageCaptureDialogModule extends Dialogs.ExtensionTemplatedDialogBase {
    private _resolve: DialogResolve;
    private _data: IImageCaptureDialogResult;
    public userEnteredValue: ko.Observable<string>;

    constructor() {
        super();
        this.userEnteredValue = ko.observable("");
    }

    /**
     * Executed when the dialog is instantiated, its HTML element is rendered and ready to be used.
     * @param {HTMLElement} element The HTMLElement
     */
    public onReady(element: HTMLElement): void {

        ko.applyBindings(this, element);

        const video: HTMLVideoElement = document.querySelector('#video') as HTMLVideoElement;
        const canvas: HTMLCanvasElement = document.querySelector('#canvas') as HTMLCanvasElement;
        const captureButton: HTMLButtonElement = document.querySelector('#capture') as HTMLButtonElement;
        const saveButton: HTMLButtonElement = document.querySelector('#capture') as HTMLButtonElement
        const capturedImage: HTMLImageElement = document.getElementById('capturedImage') as HTMLImageElement;
        const context = canvas.getContext('2d');

        navigator.mediaDevices.getUserMedia({ video: true })
            .then((stream) => {
                video.srcObject = stream;
            })
            .catch((error) => {
                console.error('Error accessing the camera:', error);
            });

        // Capture the image
        captureButton.addEventListener('click', () => {
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            // Display the captured image
            const dataURL = canvas.toDataURL('image/png');
            capturedImage.src = dataURL;
        });

        // Save the image
        saveButton.addEventListener('click', () => {
            const dataURL = canvas.toDataURL('image/png'); // Convert canvas to data URL\
            this._data = {
                imageData: dataURL,
                description: this.userEnteredValue(),
                timestamp: new Date().toISOString()
            };
            const link = document.createElement('a'); // Create a temporary <a> element
            link.href = dataURL;
            link.download = 'captured-image.png'; // Set the file name
            link.click(); // Trigger the download
        });
        
    }

    /**
     * Opens the dialog.
     * @param {Entities.ExampleEntity} dataToEdit The date to be edited through the dialog.
     */
    public open(message: string): Promise<IImageCaptureDialogResult> {
        this.userEnteredValue(message);
        this._data = {};
        let promise: Promise<IImageCaptureDialogResult> = new Promise((resolve: DialogResolve, reject: DialogReject) => {
            this._resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Capture Image",
                button1: {
                    id: "buttonUpdate",
                    label: "Update",
                    isPrimary: true,
                    onClick: this._buttonUpdateClickHandler.bind(this)
                },
                button2: {
                    id: "buttonCancel",
                    label: "Cancel",
                    onClick: this._buttonCancelClickHandler.bind(this)
                },
                onCloseX: () => this._buttonCancelClickHandler()
            };

            this.openDialog(option);
        });

        return promise;
    }

    private _buttonUpdateClickHandler(): boolean {
        this._resolvePromise(this._data);
        return true;
    }

    private _buttonCancelClickHandler(): boolean {
        this._resolvePromise(null);
        return true;
    }


    private _resolvePromise(result: IImageCaptureDialogResult): void {
        if (ObjectExtensions.isFunction(this._resolve)) {
            this._resolve(result);
            this._resolve = null;
        }
    }
}
