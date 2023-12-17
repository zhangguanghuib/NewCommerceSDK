export interface IStoreHours {
    id: number;
    weekDay: WeekDays;
    openHour: Hours;
    closeHour: Hours;
    channelId: string;
    action?: UpdateDeleteAction;
}

export interface IAvailableWeekDay {
    weekDay: WeekDays;
    displayText: string;
}

export interface IAvailableHour {
    hour: Hours;
    displayText: string;
}

/**
 * WeekDays enum type.
 */
export enum WeekDays {
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

/**
 * Hours enum type.
 */
export enum Hours {
    zero = 0,
    one = 1,
    two = 2,
    three = 3,
    four = 4,
    five = 5,
    six = 6,
    seven = 7,
    eight = 8,
    nine = 9,
    ten = 10,
    eleven = 11,
    twelve = 12,
    thirteen = 13,
    fourteen = 14,
    fifteen = 15,
    sixteen = 16,
    seventeen = 17,
    eightteen = 18,
    nineteen = 19,
    twenty = 20,
    twentyOne = 21,
    twentyTwo = 22,
    twentyThree = 23,
    twentyFour = 24
}


export interface IAction {
    action: UpdateDeleteAction;
    displayText: string;
}

export enum UpdateDeleteAction {
    Update = 0,
    Delete = 1
}
