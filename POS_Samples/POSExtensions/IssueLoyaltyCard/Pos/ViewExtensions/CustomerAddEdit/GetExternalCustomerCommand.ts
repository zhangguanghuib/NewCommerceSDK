﻿import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { IAlphanumericInputDialogOptions, ShowAlphanumericInputDialogClientResponse, ShowAlphanumericInputDialogClientRequest } from "PosApi/Consume/Dialogs";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as CustomerAddEditView from "PosApi/Extend/Views/CustomerAddEditView";

export default class GetExternalCustomerCommand extends CustomerAddEditView.CustomerAddEditExtensionCommandBase {

    /**
     * Creates a new instance of the GetExternalCustomerCommand class.
     * @param {IExtensionCommandContext<CustomerAddEditView.ICustomerAddEditToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<CustomerAddEditView.ICustomerAddEditToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "getExternalCustomerCommand";
        this.label = "Get Customer";
        this.extraClass = "iconLightningBolt";

    }

    /**
     * Initializes the command.
     * @param {CustomerAddEditView.ICustomerAddEditExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: CustomerAddEditView.ICustomerAddEditExtensionCommandState): void {
        if (state.isNewCustomer) {
            // We don't want to override a customer if it already exists.
            this.isVisible = true;
            this.canExecute = true;
        }

        //let btnSaveCustomer: HTMLElement = document.getElementById("cmdSaveCustomerDetails");
        //btnSaveCustomer.removeAttribute("data-bind");


        document.addEventListener('keypress', function (event: KeyboardEvent) {
            if (event.key === "Enter" && state) {
                // alert(event.key + " " + event.which);
                event.preventDefault();
                event.stopPropagation();
                return false;
            } else {
                return true;
            }
        });
    }

    public dispose(): void {
        console.log("Disposed");
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        let inputAlphanumericDialogOptions: IAlphanumericInputDialogOptions = {
            title: "External System",
            numPadLabel: "Please enter code:"
        };

        let dialogRequest: ShowAlphanumericInputDialogClientRequest<ShowAlphanumericInputDialogClientResponse>
            = new ShowAlphanumericInputDialogClientRequest<ShowAlphanumericInputDialogClientResponse>(inputAlphanumericDialogOptions);

        this.context.runtime.executeAsync(dialogRequest)
            .then((dialogResponse: ClientEntities.ICancelableDataResult<ShowAlphanumericInputDialogClientResponse>) => {
                if (!dialogResponse.canceled) {
                    let customer: ProxyEntities.Customer = this.customer;
                    let primaryAddress: ProxyEntities.Address = new ProxyEntities.AddressClass();

                    if (dialogResponse.data.result.value === "1") {
                        primaryAddress.AddressTypeValue = ClientEntities.ExtensibleAddressType.Business.Value;
                        primaryAddress.City = "Redmond";
                        primaryAddress.IsPrimary = true;
                        primaryAddress.Name = "Contoso Consulting USA";
                        primaryAddress.State = "WA";
                        primaryAddress.StateName = "Washington";
                        primaryAddress.Street = "454 1st Street\nSuite 99";
                        primaryAddress.ThreeLetterISORegionName = "USA";
                        primaryAddress.ZipCode = "98052";

                        let addresses: ProxyEntities.Address[] = [primaryAddress];
                        customer.CustomerTypeValue = ProxyEntities.CustomerType.Organization;
                        // Having a name is mandatory.
                        customer.Name = "Contoso Consulting USA";
                        customer.Addresses = addresses;
                    } else {
                        primaryAddress.AddressTypeValue = ClientEntities.ExtensibleAddressType.Home.Value;
                        primaryAddress.City = "Redmond";
                        primaryAddress.IsPrimary = true;
                        primaryAddress.Name = "Town house";
                        primaryAddress.State = "WA";
                        primaryAddress.StateName = "Washington";
                        primaryAddress.Street = "One microsoft way";
                        primaryAddress.ThreeLetterISORegionName = "USA";
                        primaryAddress.ZipCode = "98052";

                        let addresses: ProxyEntities.Address[] = [primaryAddress];

                        customer.CustomerTypeValue = ProxyEntities.CustomerType.Person;
                        // Having a name is mandatory.
                        customer.Name = "John Doe";
                        customer.FirstName = "John";
                        customer.LastName = "Doe";
                        customer.Addresses = addresses;
                    }

                    this.customer = customer;
                }
            });
    }
}