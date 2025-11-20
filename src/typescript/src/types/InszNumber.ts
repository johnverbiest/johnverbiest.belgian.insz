import {Sex} from "./Sex";

export class InszNumber {
    value: number;
    hasBeenValidated: boolean;
    isValid: boolean | null = null;
    isBis: boolean | null = null;
    birthDate: Date | null = null;
    birthYear: number | null = null;
    sex: Sex | null = null;

    // Overload signatures
    constructor(value: number);
    constructor(
        value: number,
        hasBeenValidated: boolean,
        isValid: boolean | null,
        isBis: boolean | null,
        birthDate: Date | null,
        birthYear: number | null,
        sex : Sex | null
    );
    constructor(value: string);

    // Single implementation
    constructor(
        value: number | string,
        hasBeenValidated: boolean = false,
        isValid: boolean | null = null,
        isBis: boolean | null = null,
        birthDate: Date | null = null,
        birthYear: number | null = null,
        sex : Sex | null = null
    ) {
        if (typeof value === "string") {
            this.value = parseInt(value, 10);
            this.hasBeenValidated = false;
            return;
        }

        // Full constructor with all fields
        this.value = value;
        this.hasBeenValidated = hasBeenValidated || false;
        this.isValid = isValid;
        this.isBis = isBis;
        this.birthDate = birthDate;
        this.birthYear = birthYear;
        this.sex = sex;
    }

    toString(): string {
        return this.value.toString().padStart(11, '0');
    }

    toFormattedString(): string | null {
        const s = this.toString();
        if (s.length !== 11) return null;
        // Format: 00.00.00-000.00
        return `${s.substring(0,2)}.${s.substring(2,4)}.${s.substring(4,6)}-${s.substring(6,9)}.${s.substring(9,11)}`;
    }
}
