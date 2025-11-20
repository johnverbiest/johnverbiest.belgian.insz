import {InszNumber} from "./InszNumber";
import {ValidationError} from "./ValidationError";

export interface InszValidationResult {
    isValid: boolean;
    inszNumber?: InszNumber;
    validationErrors: ValidationError[];
}