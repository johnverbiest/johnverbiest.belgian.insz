import {IInszValidator} from "./IInszValidator";
import {InszNumber} from "../types/InszNumber";
import {InszValidationResult} from "../types/InszValidationResult";
import {ValidationError} from "../types/ValidationError";
import {Sex} from "../types/Sex";

enum InszMode {
    Pre2000,
    Post2000
}

export class InszValidator implements IInszValidator {
    
    Validate(insz: string | number | InszNumber): InszValidationResult {
        return InszValidator.Validate(insz);
    }
    
    static Validate(inszInput: string | number | InszNumber): InszValidationResult {
        // Get the string representation of the INSZ
        const insz = this.inszInputToString(inszInput);
        
        // Validate characters
        if (!this.validateCharacters(insz)) {
            return {
                isValid: false,
                validationErrors: [ValidationError.InputIsNotANumber],
            }
        }
        
        // Validate length
        if (insz.length !== 11) {
            return {
                isValid: false,
                validationErrors: [ValidationError.InputIsWrongLength],
            }
        }
        
        // Validate checksum and determine mode
        const mode = this.validateChecksumAndReturnMode(insz);
        if (mode === null) {
            return {
                isValid: false,
                validationErrors: [ValidationError.ChecksumIsInvalid],
            }
        }
        
        // Validate date and extract birth date data
        const birthDayData = this.validateDateAndReturnBirthDayData(insz, mode);
        if (birthDayData === null) {
            return {
                isValid: false,
                validationErrors: [ValidationError.DateIsInvalid],
                inszNumber: new InszNumber(parseInt(insz), true, true, null, null, null, null)
            }
        }
        
        // Validate sequence number and parse Sex
        const sex = this.validateSequenceNumberAndParseSex(insz, birthDayData.isSexUnknown);
        if (sex === null) {
            return {
                isValid: false,
                validationErrors: [ValidationError.InvalidSequenceNumber],
                inszNumber: new InszNumber(parseInt(insz), true, false, birthDayData.isBis, birthDayData.birthDate, birthDayData.birthYear, null)
            }
        }
        
        // If all validations passed, construct the InszNumber
        const inszNumber = new InszNumber(parseInt(insz), true, true, birthDayData.isBis, birthDayData.birthDate, birthDayData.birthYear, sex);
        
        return {
            isValid: true,
            validationErrors: [],
            inszNumber: inszNumber,
        };
    }
    
    private static validateSequenceNumberAndParseSex(insz: string, isSexUnknown: boolean): Sex | null {
        const seqNumberPart = parseInt(insz.substring(6, 9));
        if (isNaN(seqNumberPart) || seqNumberPart < 1 || seqNumberPart > 998) {
            return null;
        }
        
        return isSexUnknown
            ? Sex.Unknown
            : (seqNumberPart % 2 === 0) 
                ? Sex.Female
                : Sex.Male;
    }
    
    private static validateDateAndReturnBirthDayData(insz: string, mode: InszMode): { birthDate: Date | null; isBis: boolean, birthYear: number | null, isSexUnknown: boolean } | null {
        const yearPart = insz.substring(0, 2);
        const monthPart = insz.substring(2, 4);
        const dayPart = insz.substring(4, 6);
        
        const day = parseInt(dayPart);
        const monthPlaceHolder = parseInt(monthPart);
        const yearTwoDigits = parseInt(yearPart);
        const year = mode === InszMode.Pre2000 ? 1900 + yearTwoDigits : 2000 + yearTwoDigits;
        
        const isRrn = monthPlaceHolder >= 0 && monthPlaceHolder <= 12
        const isSexKnown = isRrn || (monthPlaceHolder >= 40 && monthPlaceHolder <= 52);
        const isSexUnknown = monthPlaceHolder >= 20 && monthPlaceHolder <= 32;
        
        if (!(isSexKnown || isSexUnknown)) { // Invalid month
            return null;
        }
        
        const month = isRrn 
            ? monthPlaceHolder 
            : isSexUnknown
                ? monthPlaceHolder - 20
                : monthPlaceHolder - 40;
        
        
        // Handle partially known date (BIS)
        if (!isRrn && month == 0) { // Bis Partially known date
            if (day == 0) { // If the day is also 0, we only have a valid year
                return {
                    isBis: !isRrn,
                    isSexUnknown: isSexUnknown,
                    birthYear: year,
                    birthDate: null
                };
            } else if (yearTwoDigits == 0 && day > 0 && day <= 10) { // If the day is between 1 and 10, we don't know the year
                return {
                    isBis: !isRrn,
                    isSexUnknown: isSexUnknown,
                    birthDate: null,
                    birthYear: null,
                };
            } else { // Invalid day for bis
                return null;
            }
        }
        
        // Validate full date
        const birthDate = new Date(year, month - 1, day);
        if (birthDate.getFullYear() !== year || birthDate.getMonth() + 1 !== month || birthDate.getDate() !== day) {
            return null;
        }
        
        return {
            birthDate: birthDate,
            isBis: !isRrn,
            isSexUnknown: isSexUnknown,
            birthYear: year
        };
    }
    
    private static validateChecksumAndReturnMode(insz: string): InszMode | null {
        const numberPart = insz.substring(0, 9);
        const checksumPart = parseInt(insz.substring(9, 11), 10);
        
        const numberValue = parseInt(numberPart, 10);
        
        const pre2000Checksum = 97 - (numberValue % 97);
        if (pre2000Checksum === checksumPart) {
            return InszMode.Pre2000;
        }
        
        const post2000Checksum = 97 - ((2000000000 + numberValue) % 97);
        if (post2000Checksum === checksumPart) {
            return InszMode.Post2000;
        }
        
        return null;
    }
    
    private static validateCharacters(insz: string): boolean {
        const inszRegex = /^\d*$/;
        return inszRegex.test(insz);
    }
    
    private static inszInputToString(inszInput: string | number | InszNumber): string {
        if (typeof inszInput === 'string') {
            return inszInput
                .replace(/[ -./_]+/g, "");
        } else if (typeof inszInput === 'number') {
            return inszInput.toString().padStart(11, '0');
        } else {
            return this.inszInputToString(inszInput.value);
        }
    }
}