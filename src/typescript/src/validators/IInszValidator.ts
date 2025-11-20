import {InszNumber} from "../types/InszNumber";
import {InszValidationResult} from "../types/InszValidationResult";

export interface IInszValidator {
    Validate(insz: string|number|InszNumber) : InszValidationResult;
}