import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class FloorNavIndex extends Views.CustomViewControllerBase {
   
    public onReady(element: HTMLElement): void {
        let lis: NodeListOf<HTMLLIElement> = document.querySelectorAll("#nav li");
        let main: HTMLDivElement = document.getElementById("main") as HTMLDivElement;

        for (let i: number = 0; i < lis.length; ++i) {
            lis[i].onclick = function () {
                for (let j = 0; j < lis.length; j++) {
                    lis[j].className = '';
                }
                lis[i].className = "active";
                main.scrollTo({
                    top: i * 800,
                    behavior: "smooth"
                });
            }
        }
        let index: number = 0;

        main.addEventListener("scroll", function () {
            let scrollDis: Number = main.scrollTop;
            console.log(scrollDis);
            if (index != Math.floor((main.scrollTop + 400) / 800)) {
                index = Math.floor((main.scrollTop + 400) / 800);
                console.log(index);
                for (let i = 0; i < lis.length; ++i) {
                    lis[i].className = '';
                }
                lis[index].className = 'active'
            }
        }, false);
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

}