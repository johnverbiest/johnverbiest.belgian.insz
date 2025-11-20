import {GetVector} from "./helpers/GetVectors";
import {InszValidator} from "../src/validators/InszValidator";
import {PrintTestCase, PrintValidationResult} from "./helpers/OutputData";
import {TestCase} from "./types/TestTypes";

describe("InszValidatorTests", () => {


    function runTest(testCase: TestCase) {
        test(`${testCase.testName || 'Unnamed test'} - ${testCase.because}`, () => {
            // Print test case details
            PrintTestCase(testCase);

            // Create validator instance
            const validator = new InszValidator();

            // Run validation
            const result = validator.Validate(testCase.input);

            // Print validation Result
            PrintValidationResult(result);

            // Assert based on expected values
            if (testCase.expected.isValid !== undefined) {
                expect(result.isValid).toBe(testCase.expected.isValid);
            }
        });
    }

    describe("Pre-2000 RN Vectors", () => {
        GetVector('pre2000-rn-tests.json').testCases.forEach((testCase) => runTest(testCase))
    });

    describe("Post-2000 RN Vectors", () => {
        GetVector('post2000-rn-tests.json').testCases.forEach((testCase) => runTest(testCase))
    });

    describe("Pre-2000 BIS Vectors", () => {
        GetVector('pre2000-bis-tests.json').testCases.forEach((testCase) => runTest(testCase))
    });

    describe("Post-2000 BIS Vectors", () => {
        GetVector('post2000-bis-tests.json').testCases.forEach((testCase) => runTest(testCase))
    });
    
});