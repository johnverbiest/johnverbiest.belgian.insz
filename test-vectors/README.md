# test-vectors

This folder contains a set of shared json test vectors for Belgian INSZ/RN/BIS identifiers.

## Schema

```json
{
  "name": "validator tests",
  "description": "High-level tests for the INSZ validator covering RN and BIS, with multiple expected outcomes (boolean and categorical).",
  "testCases": [
    {
      "input": "85061300180",
      "expected": {
        "isValid": true,
        "sex": "M"
      },
      "because": "Valid RN (male), checksum correct, pre-2000 rule."
    }
  ]
}
```

🧠 Rules:

- input: always the INSZ string to test
- expected: object with one or more key/value pairs you want to verify
  - e.g. "isValid", "sex", "isBis", "birthDate", "century", …
- each key is optional per test — only list the properties you want checked
- values can be:
  - boolean — true/false
  - string — e.g. "M", "F", "U", or date "YYYY-MM-DD"
  - number — e.g. 1900 or 2000
- because: human explanation (useful in docs and test error messages)
