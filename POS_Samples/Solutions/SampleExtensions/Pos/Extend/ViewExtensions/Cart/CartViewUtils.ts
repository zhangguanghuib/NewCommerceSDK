export default class CartViewUtils {

    public static setStyle(div: HTMLDivElement, styleJson: Object): void {

        Object.keys(styleJson).forEach(key => {
            div.style[key] = styleJson[key];
        });
    }
}