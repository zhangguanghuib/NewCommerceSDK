export default class Waiter {
    async waitOneSecond(): Promise<void> {
        await new Promise<void>(resolve => setTimeout(resolve, 1000));
        console.log('Waited 1 second');
    }
}