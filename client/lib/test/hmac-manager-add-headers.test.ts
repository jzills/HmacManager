import { assert, test } from "vitest";
import HmacManager from "../src/hmac-manager";
import { HashAlgorithm } from "../src/hash-algorithm";
import { HmacAuthenticationDefaults } from "../src/hmac-authentication-defaults";
import HmacHeaderBuilder from "../src/builders/hmac-header-builder";

test("HmacManager_Sign_Adds_Authorization_Header", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const hmacManager = new HmacManager({
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: HashAlgorithm.SHA256,
        signatureHashAlgorithm: HashAlgorithm.SHA256,
        schemes: []
    }, null, new HmacHeaderBuilder());

    const signingResult = await hmacManager.sign(request);
    const authorizationHeader = request.headers.get(HmacAuthenticationDefaults.Headers.Authorization);
    const authorizationHeaderSignature = authorizationHeader?.split(`${HmacAuthenticationDefaults.AuthenticationScheme} `)[1];
    assert.equal(signingResult.hmac?.signature, authorizationHeaderSignature);
});

test("HmacManager_Sign_Adds_DateRequested_Header", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const hmacManager = new HmacManager({
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: HashAlgorithm.SHA256,
        signatureHashAlgorithm: HashAlgorithm.SHA256,
        schemes: []
    }, null, new HmacHeaderBuilder());

    const signingResult = await hmacManager.sign(request)
    const dateRequestedHeader = request.headers.get(HmacAuthenticationDefaults.Headers.DateRequested);
    assert.equal(signingResult.hmac?.dateRequested.getTime().toString(), dateRequestedHeader);
});

test("HmacManager_Sign_Adds_Nonce_Header", async () => {
    const request = new Request("https://localhost:7216/api/weatherforecast");
    const hmacManager = new HmacManager({
        name: "Policy-A",
        publicKey: "eb8e9dae-08bd-4883-80fe-1d9a103b30b5",
        privateKey: btoa("thisIsMySuperCoolPrivateKey"),
        contentHashAlgorithm: HashAlgorithm.SHA256,
        signatureHashAlgorithm: HashAlgorithm.SHA256,
        schemes: []
    }, null, new HmacHeaderBuilder());

    const signingResult = await hmacManager.sign(request)
    const nonceHeader = request.headers.get(HmacAuthenticationDefaults.Headers.Nonce);
    assert.equal(signingResult.hmac?.nonce, nonceHeader);
});