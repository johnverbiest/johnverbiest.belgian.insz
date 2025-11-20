import {TestVector} from "../types/TestTypes";
import * as fs from 'fs';
import * as path from 'path';

export const GetVector = (filename: string) : TestVector => {
    const testVectorPath = path.join(__dirname, '../../../../test-vectors/', filename);
    const testVectorData = fs.readFileSync(testVectorPath, 'utf8');
    return JSON.parse(testVectorData);
}