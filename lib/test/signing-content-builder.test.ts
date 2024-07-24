import { expect, test } from "vitest";
import { SigningContentBuilder } from "../hmac_manager/builders/signing-content-builder.js";
import { equal, fail } from "assert";
import { computeContentHash } from "../hmac_manager/utilities/hmac-utilities.js";
import { HmacManager } from "../hmac_manager/hmac-manager.js";

test("HmacManager", async () => {
    const request = new Request("https://localhost:44363", {
        method: "post",
        headers: {
            "X-Account": "123",
            "X-Email": "my@email.com"
        }
    });

    const publicKey = "eb8e9dae-08bd-4883-80fe-1d9a103b30b5";
    const privateKey = btoa("thisIsMySuperCoolPrivateKey");
    const signedHeaders = ["X-Account", "X-Email"];
    const hmacManager = new HmacManager(publicKey, privateKey, signedHeaders);
    const signingResult = await hmacManager.sign(request);

    expect(equal(signingResult.isSuccess, true));
    console.log(signingResult.hmac);
    expect(equal(signingResult.hmac?.signature, "l7sx2TUu5c6CsoXxG8yqMZ035XMYx95iLdHExAa6KgE="));
})