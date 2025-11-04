# test-vectors

This folder contains a set of shared json test vectors for Belgian INSZ/RN/BIS identifiers.

## Schema

```json
{
  "name": "pre-2000 RN validator tests",
  "description": "Working and failing Belgian Rijksregisternummer (pre-2000 rule only). No BIS or special cases included; all months are 01–12.",
  "testCases": [
    {
      "testName": "short label",
      "testDescription": "concise explanation of what this case verifies",
      "input": "85061300178",
      "expected": { "isValid": true },
      "because": "Detailed reasoning if you need to explain why the outcome is expected"
    }
  ]
}
```

🧠 Rules:

- testName: human-readable name for the test case
- testDescription: longer human-readable description of what the test case verifies
- input: always the INSZ string to test
- expected: object with one or more key/value pairs you want to verify
  - e.g. "isValid", "sex", "isBis", "birthDate", "century", …
- each key is optional per test — only list the properties you want checked
- values can be:
  - boolean — true/false
  - string — e.g. "M", "F", "U", or date "YYYY-MM-DD"
  - number — e.g. 1900 or 2000
- because: human explanation (useful in docs and test error messages)
