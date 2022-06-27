import * as ClientStoreHours from "../Entities/IStoreHours";
import { StringExtensions } from "PosApi/TypeExtensions";
import { Entities } from "../DataService/DataServiceEntities.g";


export default class StoreHourConverter {
    public static getWeekdayName(dayOfWeek: ClientStoreHours.WeekDays): string {
        let weekDayName: string = "";

        switch (dayOfWeek) {
            case ClientStoreHours.WeekDays.Monday:
                weekDayName = "Monday";
                break;
            case ClientStoreHours.WeekDays.Tuesday:
                weekDayName = "Tuesday";
                break;
            case ClientStoreHours.WeekDays.Wednesday:
                weekDayName = "Wednesday";
                break;
            case ClientStoreHours.WeekDays.Thursday:
                weekDayName = "Thursday";
                break;
            case ClientStoreHours.WeekDays.Friday:
                weekDayName = "Friday";
                break;
            case ClientStoreHours.WeekDays.Saturday:
                weekDayName = "Saturday";
                break;
            case ClientStoreHours.WeekDays.Sunday:
                weekDayName = "Sunday";
                break;
            default: break;
        }

        return weekDayName;
    }

    public static formatStoreHour(hour: ClientStoreHours.Hours): string {
        let strHour: string = "00:00";

        if (hour < ClientStoreHours.Hours.ten) {
            strHour = StringExtensions.format("0{0}:00", hour.toString());
        } else {
            strHour = hour.toString() + ":00";
        }

        return strHour;
    }

    public static convertToClientStoreHours(storeHours: Entities.StoreDayHours): ClientStoreHours.IStoreHours {
        return {
            id: storeHours.Id,
            weekDay: storeHours.DayOfWeek,
            openHour: storeHours.OpenTime / 3600,
            closeHour: storeHours.CloseTime / 3600
        };
    }

    public static convertToServerStoreHours(storeHours: ClientStoreHours.IStoreHours): Entities.StoreDayHours {
        return new Entities.StoreDayHours({
            Id: storeHours.id,
            DayOfWeek: storeHours.weekDay,
            OpenTime: storeHours.openHour * 3600,
            CloseTime: storeHours.closeHour * 3600
        });
    }
}