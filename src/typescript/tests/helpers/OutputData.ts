import {TestCase} from "../types/TestTypes";
import {InszValidationResult} from "../../src/types/InszValidationResult";


export const PrintTestCase = (vector: TestCase): void => {
    const output = `Test Case:` +
        `\n  Test Name: ${vector.testName || 'Unnamed test'}` +
        `\n  Input: ${vector.input}` +
        `\n  Because: ${vector.because}`;
    console.info(output);
};

export const PrintValidationResult = (result: InszValidationResult): void => {
    const output = `Validation Result:` +
        `\n  isValid: ${result.isValid}` +
        `\n  validationErrors: [${result.validationErrors.join(', ')}]` +
        `\n  inszNumber: ${result.inszNumber ? JSON.stringify(result.inszNumber) : 'undefined'}`;
    console.info(output);
}