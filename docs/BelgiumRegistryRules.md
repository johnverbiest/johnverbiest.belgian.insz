# Belgium Registry Rules

Authoritative rules for validating and parsing the Belgian **INSZ** identifiers:
- **RN** – *Rijksregisternummer* for persons registered in the National Register.
- **BIS** – number assigned by the **Kruispuntbank van de Sociale Zekerheid (KSZ/BCSS)** for persons **not** registered in the National Register.

All references below point to **official Belgian government sources** (IBZ/FOD Binnenlandse Zaken, KSZ/BCSS, and the eHealth platform).

> ⚠️ **Privacy/GDPR**: INSZ/RN/BIS are special personal identifiers under Belgian law. Store and process them only when you have a lawful basis and strictly minimize exposure in logs, analytics, and test datasets.

---

## 1) Scope and terminology

- **INSZ / NISS** is the umbrella identifier used across social security and eHealth. It always has **11 digits**. For people in the National Register, the INSZ **equals the RN**; for others, the INSZ is a **BIS** number assigned by KSZ.  
  **Source:** eHealth (NISS INSZ) — https://www.ehealth.fgov.be/nl/page/niss-insz

- Within social security, the INSZ is the “sleutel” (key) for data exchange; it is either an RN or a BIS.  
  **Source:** KSZ/BCSS — Rijksregister/KSZ-registers — https://www.ksz-bcss.fgov.be/nl/project/rijksregisterksz-registers

---

## 2) RN (Rijksregisternummer) structure and allocation

**Layout (digits only)**: `YYMMDDXXXCC` (separators like `.` or `-` may appear on printed media, but the number itself is 11 digits).

From the IBZ/rrn official instruction **IT000 – Het Identificatienummer**:

- The RN contains **11 digits**:  
  - First group (`YYMMDD`) = birthdate (year, month, day).  
  - Second group (`XXX`) = sequence number for persons born on the same day; **even ⇒ female**, **odd ⇒ male**; allocated **001–997 (male)** and **002–998 (female)**; `000` and `999` are *not* assigned.  
  - Third group (`CC`) = control/check digits computed from the first nine digits.  
  **Source:** IBZ (IT000) — https://www.ibz.rrn.fgov.be/sites/default/files/documents/nl/rijksregister/onderrichtingen/IT-lijst/IT000_Rijksregisternummer.pdf (pp. 1–2)

- **Checksum (modulo‑97) for RN**:  
  Let `base9 = YYMMDDXXX` as an integer.  
  Compute `CC = 97 − (base9 % 97)`.  
  For **births from 1‑Jan‑2000 onwards**, compute the checksum **as if a leading `2` is prefixed** to the nine digits (conceptually: `2000000000 + base9` before taking `% 97`).  
  **Source:** IBZ (IT000) — same PDF, “overgang naar het jaar 2000” (pp. 3–4)

- **Century inference** (RN): if the entered 11 digits pass the checksum **without** the `2`‑prefix computation, treat the century as **1900s**; if it only passes **with** the `2`‑prefix computation, treat it as **2000s**.  
  **Source:** IBZ (IT000) — same PDF

---

## 3) BIS number structure and special rules

The BIS number is also **11 digits** and follows the same overall layout `YYMMDDXXXCC`, but with specific **month encodings** and the same modulo‑97 checksum rule. The **official legal basis** is the **Koninklijk besluit van 8 februari 1991**.

- **Composition** (Article 1): 11 digits; first six are the birth date (adjusted), then a three‑digit series (`XXX`), then a two‑digit control number (`CC`).  
  **Source:** KSZ/BCSS — KB 8 Feb 1991 — https://www.ksz-bcss.fgov.be/nl/page/koninklijk-besluit-van-8-februari-1991

- **Month encoding** (Article 2):  
  - If **sex is known at issuance** ⇒ `MM = realMonth + 40` ⇒ allowed month values **41–52**.  
  - If **sex is unknown at issuance** ⇒ `MM = realMonth + 20` ⇒ allowed month values **21–32**.  
  **Source:** KSZ/BCSS — KB 8 Feb 1991 — same page

- **Series number & parity** (Article 3): if sex is known at issuance, **even ⇒ female**, **odd ⇒ male**.  
  **Source:** KSZ/BCSS — KB 8 Feb 1991 — same page

- **Checksum** (Article 4): same modulo‑97 rule as RN: `97 − (base9 % 97)`. For persons born **in or after 2000**, the decree explicitly states that the calculation is performed **“by preceding the nine digits with the digit 2”** (i.e., the 2000+ rule).  
  **Source:** KSZ/BCSS — KB 8 Feb 1991 — same page (Article 4, amendment note)

- **Unknown date encodings** (Article 5): when day/month (or year) are unknown at issuance, specific placeholder patterns are used (e.g., the third digit of the date block set to `4` or `2` depending on known/unknown sex, with zeroes for the unknown components). When series capacity is exhausted, the decree prescribes incrementing the last date digit and restarting the series.  
  **Source:** KSZ/BCSS — KB 8 Feb 1991 — same page

> **Note**: Because of the Article 5 fallbacks, **BIS** may not definitively encode the true birthdate/sex. Implementations should expose BIS date/sex only as “best effort” inferences.

---

## 4) INSZ overview in government portals

- **eHealth**: confirms **INSZ = 11 digits**; it equals the RN when available, otherwise it’s a KSZ/BCSS (BIS) number.  
  **Source:** eHealth — https://www.ehealth.fgov.be/nl/page/niss-insz

- **RSZ/SocialSecurity** (Dimona docs): shows the structure `JJMMDD-sss-CG`, parity of `sss` (odd male/even female), and **month +20/+40 for BIS**. Advises that the INSZ should be entered **as 11 digits without separators**.  
  **Source:** RSZ/SocialSecurity — https://www.socialsecurity.be/site/v2/dimona/nl/dimona/scenario/fields/action-insz

---

## 5) Implementation checklist (for validators/parsers)

1. **Normalize**: strip any `.`/`-`/spaces; ensure **exactly 11 digits**. (RSZ/SocialSecurity advises 11 digits, no separators.)  
   Source: RSZ/SocialSecurity — link above.

2. **Split**: `YYMMDD` (first 6), `XXX` (next 3), `CC` (last 2).

3. **Detect BIS vs RN**:  
   - If `MM` in **21–32** ⇒ BIS with **sex unknown at issuance** (`realMonth = MM − 20`).  
   - If `MM` in **41–52** ⇒ BIS with **sex known at issuance** (`realMonth = MM − 40`).  
   - Otherwise treat as RN.

4. **Date rules**:  
   - RN: `YYMMDD` must be a valid calendar date once the **century is inferred via the checksum rule** (see step 6).  
   - BIS: **reversed month offset** yields candidate month; day may be administratively encoded per Article 5 — if not a real day, treat as **administrative/fictitious** and do *not* expose a definitive `BirthDate` (only an optional `BirthDateIfEncodable`).

5. **Sequence (`XXX`)**:  
   - RN: **001–997 (male)** and **002–998 (female)**; `000` and `999` are invalid. Parity must match inferred sex.  
     Source: IBZ (IT000), pp. 1–2.  
   - BIS: allocation counter; if sex known at issuance (Art. 3), parity rule applies. `000` and `999` are not used.

6. **Checksum** (always modulo‑97 on first nine digits):  
   - Compute `cc_pre2000 = 97 − (base9 % 97)` and `cc_post2000 = 97 − ((2_000_000_000 + base9) % 97)`.  
   - Accept if either matches `CC`. For **RN**, the matching branch implies the **century** (1900s vs 2000s).  
   - Sources: IBZ (IT000) and KSZ decree (KB 8 Feb 1991, Art. 4).

7. **Output/inference**:  
   - `IsValid`, `IsBis`, `Sex` (`Male`, `Female`, `Unknown`), `BirthDate` (RN only), `BirthDateIfEncodable` (nullable for BIS/RN), `HasAdministrativeDay` (true when day is an administrative placeholder per Art. 5), and `Century` when inferable.

---

## 6) Examples (illustrative)

> **These are synthetic formats** to illustrate structure and checks; do **not** copy real identifiers.

- **RN, born 1994‑02‑13, male**  
  `940213 001 CC` → parity odd ⇒ male; compute `CC` with **pre‑2000** rule.

- **RN, born 2006‑07‑05, female**  
  `060705 002 CC` → parity even ⇒ female; compute `CC` with **post‑2000 (“+2”)** rule.

- **BIS, sex known at issuance, date 1985‑09‑30**  
  Month encoded as `09 + 40 = 49` ⇒ `850930 0x? CC` (odd/even per Art. 3).

- **BIS, sex unknown at issuance, date 1972‑03‑??**  
  Month encoded as `03 + 20 = 23`; day may be `00` per Art. 5 fallback.

---

## 7) Official references (primary sources)

1) **IBZ / Rijksregister — IT000 (Het Identificatienummer)** — RN structure, sequence ranges, modulo‑97, and 2000+ rule  
   https://www.ibz.rrn.fgov.be/sites/default/files/documents/nl/rijksregister/onderrichtingen/IT-lijst/IT000_Rijksregisternummer.pdf

2) **KSZ/BCSS — Koninklijk besluit van 8 februari 1991** — BIS composition, month +20/+40, parity when sex known, modulo‑97, and the 2000+ (“prefix 2”) rule  
   https://www.ksz-bcss.fgov.be/nl/page/koninklijk-besluit-van-8-februari-1991

3) **eHealth — NISS/INSZ** — government overview of INSZ (11 digits; RN vs BIS)  
   https://www.ehealth.fgov.be/nl/page/niss-insz

4) **RSZ/SocialSecurity — Dimona INSZ field** — structure `JJMMDD-sss-CG`, BIS month offsets (+20/+40), parity of `sss`, and “digits only” entry advice  
   https://www.socialsecurity.be/site/v2/dimona/nl/dimona/scenario/fields/action-insz

---

## 8) Notes and edge cases

- **No reuse of assigned numbers** (RN/BIS). RN: see IT000 (and subsequent K.B. updates cited therein). BIS: Art. 6 (KB 8 Feb 1991 — modified 13 Oct 1998).  
- **Unknown dates** (BIS Art. 5): permit placeholder encodings; do **not** assert a real civil birth date.  
- **Sequence exhaustion**: both RN (IT000 examples) and BIS (Art. 5) prescribe increment/restart patterns when series run out.  
- **Formatting**: machine exchange generally uses 11 digits; separators are present in human‑readable prints only (see RSZ/SocialSecurity guidance).

---

*Prepared for the `Be.Identifiers.NationalNumber` package as a normative rule companion, with citations restricted to official Belgian government sources.*
