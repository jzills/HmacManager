import { assert, test } from "vitest";
import { HmacManagerFactory } from "../hmac_manager/hmac-manager-factory.js";

test("HmacManagerFactory", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast", {
        headers: {
            "X-AccountId": "123",
            "X-Email": "my@email.com"
        }
    });

    const hmacManagerFactory = new HmacManagerFactory([{
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: "sha-256",
        signatureHashAlgorithm: "sha-256",
        schemes: [{
            name: "Scheme",
            headers: ["X-AccountId", "X-Email"]
        }]
    }]);

    const hmacManager = hmacManagerFactory.create("Policy-A");
    await hmacManager?.sign(request);

    const policyHeader = request.headers.get("X-Hmac-Policy");
    const schemeHeader = request.headers.get("X-Hmac-Scheme");
    assert.equal(policyHeader, "Policy-A");
    assert.equal(schemeHeader, null);
});