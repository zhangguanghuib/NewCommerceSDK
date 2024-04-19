import * as Triggers from "PosApi/Extend/Triggers/TransactionTriggers";

export default class PostVoidTransactionTrigger extends Triggers.PostVoidTransactionTrigger {

    public execute(options: Triggers.IPostVoidTransactionTriggerOptions): Promise<void> {
        const button: HTMLButtonElement | null = document.querySelector('button[data-action="401"]');

        if (button) {
            // Add an event listener to the button
            button.addEventListener('click', () => {
                console.log('Button with data-action="401" clicked!');
            });

            // Trigger the click event programmatically
            button.click();
        } else {
            console.error('Button with data-action="401" not found.');
        }

        return Promise.resolve();

    }

}