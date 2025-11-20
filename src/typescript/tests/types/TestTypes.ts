import {Sex} from "../../src/types/Sex";

export interface Expected {
    isValid?: boolean;
    birthDate?: string;
    birthYear?: number;
    isBis?: boolean;
    sex?: string;
}

export const ToSexEnum = (sexString: string | undefined) : Sex | null => 
    !sexString 
        ? null 
        : sexString === "U"
            ? Sex.Unknown
            : sexString === "M" ? Sex.Male : Sex.Female;
            

export interface TestCase {
    testName?: string;
    testDescription?: string;
    input: string;
    expected: Expected;
    because: string;
    name?: string;
    description?: string;
}

export interface TestVector {
    name: string;
    description: string;
    testCases: TestCase[];
}
