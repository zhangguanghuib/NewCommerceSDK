import { ClientEntities } from "PosApi/Entities";
import { GetShippingDateClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { GetShippingDateClientRequest, GetShippingDateClientResponse } from "PosApi/Consume/Cart";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { Entities } from "../../DataService/DataServiceEntities.g";
import { DlvModeBookSlot } from "../../DataService/DataServiceRequests.g";

export default class GetShippingDateClientRequestHandlerExt extends GetShippingDateClientRequestHandler {

    private shippingMethodBookingSlotId: string = 'shippingMethodBookingSlot___';
    /**
     * Executes the request handler asynchronously.
     * @param {GetShippingDateClientRequest<GetShippingDateClientResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<GetShippingDateClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: GetShippingDateClientRequest<GetShippingDateClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetShippingDateClientResponse>> {

        let shippingMethodDiv: HTMLDivElement = document.querySelector('div.section[aria-label="Shipping method"][data-bind*="sectionWrapper: { headerResx: \'string_2504\' }"]');
        console.log(shippingMethodDiv);
        let shippingMethodsSection: HTMLDivElement = shippingMethodDiv.querySelector(`div:nth-child(2)`) as HTMLDivElement;
        console.log(shippingMethodsSection);

        return this.context.runtime.executeAsync(new DlvModeBookSlot.GetDlvModeBookSlotsRequest<DlvModeBookSlot.GetDlvModeBookSlotsResponse>(request.deliveryMethod.Code))
            .then((result: ClientEntities.ICancelableDataResult<DlvModeBookSlot.GetDlvModeBookSlotsResponse>): Promise<void> => {
                let bookSLots: Entities.DlvModeBookSlot[] = result.data.result;
                console.log(bookSLots);
                // Build your table here
                return Promise.resolve();
            }).catch((reason: any) => {
                this.context.logger.logError("ShippingMethods: " + JSON.stringify(reason));
            }).then(() => {
                let testTable = this.createTable();
                let shippingMethodsList = shippingMethodsSection.querySelector(`div:nth-child(2)`) as HTMLDivElement;;
                shippingMethodsList.appendChild(testTable);
                return this.defaultExecuteAsync(request);
            });
    }

    private createTable(): HTMLDivElement {

        let shippingMethodBookingSlot: HTMLDivElement = document.getElementById(this.shippingMethodBookingSlotId) as HTMLDivElement;
        if (!ObjectExtensions.isNullOrUndefined(shippingMethodBookingSlot)) {
            shippingMethodBookingSlot.remove();
        }
        shippingMethodBookingSlot = document.createElement('div') as HTMLDivElement;
        shippingMethodBookingSlot.id = this.shippingMethodBookingSlotId;
        // shippingMethodBookingSlot.textContent = 'testAAAAAAA####';

        let table: HTMLTableElement = document.createElement('table') as HTMLTableElement;

        // Add some rows and cells
        for (let i = 0; i < 3; i++) {
            let tr: HTMLTableRowElement = document.createElement('tr') as HTMLTableRowElement;
            for (let j = 0; j < 3; j++) {
                let td: HTMLTableCellElement = document.createElement('td') as HTMLTableCellElement;
                td.textContent = `Row${i + 1}-Col${j + 1}`;
                tr.appendChild(td);
            }
            table.appendChild(tr);
        }

        // Style the table
        table.style.width = '100%';
        table.style.borderCollapse = 'collapse';
        table.style.fontFamily = 'Arial, sans-serif';
        table.style.fontSize = '14px';
        table.style.color = 'white';
        table.style.border = '1px solid #dddddd';
        table.style.textAlign = 'left';
        table.style.backgroundColor = 'blue';

        // Style the table cells
        var cells = table.getElementsByTagName('td');
        for (let k = 0; k < cells.length; k++) {
            cells[k].style.border = '1px solid #dddddd';
            cells[k].style.padding = '8px';
        }

        shippingMethodBookingSlot.appendChild(table);

        return shippingMethodBookingSlot;  
    }

}
