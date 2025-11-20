import {GetVector} from "./helpers/GetVectors";
import {InszValidator} from "../src/validators/InszValidator";
import {PrintTestCase, PrintValidationResult} from "./helpers/OutputData";
import {TestCase, ToSexEnum} from "./types/TestTypes";

describe("InszNumberTests", () => {
    function formatDateYYYYMMDD(date?: Date | null): string | null {
        if (!date) return null;
        const y = date.getFullYear();
        const m = String(date.getMonth() + 1).padStart(2, '0');
        const d = String(date.getDate()).padStart(2, '0');
        return `${y}-${m}-${d}`;
    }
    
    function runTest(testCase: TestCase) {
        
        test(`${testCase.testName || 'Unnamed test'} - ${testCase.because} [BirthDate]`, () => {
            // Print test case details
            PrintTestCase(testCase);

            // Create validator instance
            const validator = new InszValidator();

            // Run validation
            const result = validator.Validate(testCase.input);

            // Print validation Result
            PrintValidationResult(result);

            // Assert based on expected values
            if (testCase.expected && testCase.expected.isValid) {
                expect(result.inszNumber).not.toBeNull();
                if (!result.inszNumber) throw result; // TypeScript type guard
                expect(formatDateYYYYMMDD(result.inszNumber.birthDate)).toBe(testCase.expected.birthDate);
            } else {
                expect(result.isValid).toBe(false);
            }
        });



        test(`${testCase.testName || 'Unnamed test'} - ${testCase.because} [BirthYear]`, () => {
            // Print test case details
            PrintTestCase(testCase);

            // Create validator instance
            const validator = new InszValidator();

            // Run validation
            const result = validator.Validate(testCase.input);

            // Print validation Result
            PrintValidationResult(result);

            // Assert based on expected values
            if (testCase.expected && testCase.expected.isValid) {
                expect(result.inszNumber).not.toBeNull();
                if (!result.inszNumber) throw result; // TypeScript type guard
                expect(result.inszNumber.birthYear).toBe(testCase.expected.birthYear ?? null);
            } else {
                expect(result.isValid).toBe(false);
            }
        });

        test(`${testCase.testName || 'Unnamed test'} - ${testCase.because} [IsBis]`, () => {
            // Print test case details
            PrintTestCase(testCase);

            // Create validator instance
            const validator = new InszValidator();

            // Run validation
            const result = validator.Validate(testCase.input);

            // Print validation Result
            PrintValidationResult(result);

            // Assert based on expected values
            if (testCase.expected && testCase.expected.isValid) {
                expect(result.inszNumber).not.toBeNull();
                if (!result.inszNumber) throw result; // TypeScript type guard
                expect(result.inszNumber.isBis).toBe(testCase.expected.isBis ?? null);
            } else {
                expect(result.isValid).toBe(false);
            }
        });

        test(`${testCase.testName || 'Unnamed test'} - ${testCase.because} [Sex]`, () => {
            // Print test case details
            PrintTestCase(testCase);

            // Create validator instance
            const validator = new InszValidator();

            // Run validation
            const result = validator.Validate(testCase.input);

            // Print validation Result
            PrintValidationResult(result);

            // Assert based on expected values
            if (testCase.expected && testCase.expected.isValid) {
                expect(result.inszNumber).not.toBeNull();
                if (!result.inszNumber) throw result; // TypeScript type guard
                expect(result.inszNumber.sex).toBe(ToSexEnum(testCase.expected.sex));
            } else {
                expect(result.isValid).toBe(false);
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