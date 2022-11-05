import ko from "knockout";
import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";
import { ObjectExtensions } from "PosApi/TypeExtensions";


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
        ObjectExtensions.disposeAllProperties(this);
    }

    public selectRandomItemsById(): void {
        let items = this._getData();

        if (items.length == 0) {
            return;
        }

        let index = Math.floor(Math.random() * items.length);
        let item = items[index];

        document.getElementById("selectByIdMessage").innerHTML = `Tried to select item with id=${item.requestId}`;
        this.paginatedDataList.selectItems([item]);
    }

    public selectItemsByName(): void {
        let nameFilterValue = (<HTMLInputElement>document.getElementById("nameFilter")).value;
        let items = this._getData();
        let itemsToSelect = items.filter(item =>
            item.requestedWorkerName.toLocaleLowerCase().indexOf(nameFilterValue.toLowerCase()) != -1);

        this.paginatedDataList.selectItems(itemsToSelect);
    }

    private _dataListSelectionChanged(): void {
        this.context.logger.logInformational("_dataListSelectionChanged");
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        document.getElementById("selectByIdMessage").innerHTML = "Try to click the 'Select random irem by id' button";
        document.getElementById("selectRandomItemsByIdButton").addEventListener('click', () => {
            this.selectRandomItemsById();
        });

        document.getElementById("selectItemsByNameButton").addEventListener('click', () => {
            this.selectItemsByName();
        });

        let paginatedDataSource: Controls.IPaginatedDataSource<IShiftData> = {
            pageSize: 5,
            loadDataPage: this._loadDataPage.bind(this),
        }
    }

    private _loadDataPage(size: number, skip: number): Promise<IShiftData[]> {
        let promise: Promise<any> = new Promise((resolve: (value?: any) => void) => {
            setTimeout(() => {
                this.context.logger.logInformational("dataListPageLoaded");
                let pageData: IShiftData[] = this._getData(size, skip);
                resolve(pageData)
            }, 1000);
        });
        return promise;
    }

    private _getData(size?: number, skip?: number): IShiftData[] {
        if (ObjectExtensions.isNullOrUndefined(skip)) {
            skip = 0;
        }

        let data: IShiftData[] = [];
        if (this._isUsingAlternativeData) {
            data = this._getAlternativeData();
        } else {
            data = this._getAllItems();
        }

        if (ObjectExtensions.isNullOrUndefined(size)) {
            return data.slice(skip);
        } else {
            return data.slice(skip, skip + size);
        }
    }

    private _getAlternativeData(): IShiftData[] {
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