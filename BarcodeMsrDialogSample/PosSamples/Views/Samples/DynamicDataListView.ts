import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";

export interface IShiftData {
    requestId: number,
    requestDateTime: Date,
    requestedWorkerName: string;
    requestType: string;
    requestStatus: string;
}

export default class DynamicDataListView extends Views.CustomViewControllerBase {

    public paginatedDataList: Controls.IPaginatedDataList<IShiftData>;

    private _isUsingAlternativeData: boolean;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "Dynamic DataList sample";
        this._isUsingAlternativeData = false;
    }

    public dispose(): void {
        throw new Error("Method not implemented.");
    }


    public onReady(element: HTMLElement): void {
        throw new Error("Method not implemented.");
    }

    private _getAllItems(): IShiftData[] {
        return [{
            requestId: 1,
            requestDateTime: new Date(),
            requestedWorkerName: "Alternative Name.",
            requestType: "Alternative Type.",
            requestStatus: "Approved"
        }];
    }

    private _getAllItems(): IShiftData[] {
        return [{
            requestId: 1,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 2,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 3,
            requestDateTime: new Date(),
            requestedWorkerName: "Greg Marchievsky",
            requestType: "Shift Offer",
            requestStatus: "Rejected"
        },
        {
            requestId: 4,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 5,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 6,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 7,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 8,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 9,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 10,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 11,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 12,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 13,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 14,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 15,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 16,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 17,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 18,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 19,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 20,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 21,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 22,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 23,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 24,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 25,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 26,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 27,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 28,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        },
        {
            requestId: 29,
            requestDateTime: new Date(),
            requestedWorkerName: "Allison Berker",
            requestType: "Leave request",
            requestStatus: "Approved"
        },
        {
            requestId: 30,
            requestDateTime: new Date(),
            requestedWorkerName: "Bert Miner",
            requestType: "Shift Swap",
            requestStatus: "Pending"
        }];
    }
}