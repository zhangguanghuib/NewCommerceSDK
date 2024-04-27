import * as Triggers from "PosApi/Extend/Triggers/TransactionTriggers";

declare var Commerce: any;
export default class PostVoidTransactionTrigger extends Triggers.PostVoidTransactionTrigger {

    public execute(options: Triggers.IPostVoidTransactionTriggerOptions): Promise<void> {
        const button: HTMLButtonElement | null = document.querySelector('button[data-action="401"]');

        if (button) {
            button.addEventListener('click', () => {
                console.log('Button with data-action="401" clicked!');
            });

            button.click();
        } else {
            console.error('Button with data-action="401" not found.');
        }

        return Promise.resolve();
    }

}