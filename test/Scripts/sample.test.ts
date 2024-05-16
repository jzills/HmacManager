import { expect, test } from "vitest";
import { Hmac } from "../../src/Scripts/HmacManager/components/hmac.js"

test("Sample", () => {
    expect(new Hmac().headerValues.length).toBe(0)
})