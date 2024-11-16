import { assert, test } from "vitest";
import HmacHeaderBuilder from "../hmac_manager/builders/hmac-header-builder.js";

test("HmacHeaderBuilder_Build_ExpectOptionsNull", () => {
    const authorization = "someAuthorizationHeaderValue";
    const policy = "Some Policy";
    const scheme = "Some Scheme";
    const dateRequested = new Date();
    const nonce = "Some Nonce";

    const builder = new HmacHeaderBuilder()
        .withAuthorization(authorization)
        .withPolicy(policy)
        .withScheme(scheme)
        .withDateRequested(dateRequested)
        .withNonce(nonce);
    
    const headers = builder.build();
    assert.equal(headers.Authorization, `Hmac ${authorization}`);
    assert.equal(headers["Hmac-Policy"], policy);
    assert.equal(headers["Hmac-Scheme"], scheme);
    assert.equal(headers["Hmac-DateRequested"], dateRequested.getTime().toString());
    assert.equal(headers["Hmac-Nonce"], nonce);
    assert.equal(headers["Hmac-Options"], null);
});

test("HmacHeaderBuilder_Build_ExpectAllNull", () => {
    const headers = new HmacHeaderBuilder().build();
    assert.equal(headers.Authorization, null);
    assert.equal(headers["Hmac-Policy"], null);
    assert.equal(headers["Hmac-Scheme"], null);
    assert.equal(headers["Hmac-DateRequested"], null);
    assert.equal(headers["Hmac-Nonce"], null);
    assert.equal(headers["Hmac-Options"], null);
});

test("HmacHeaderBuilder_Build_ExpectSchemeNull", () => {
    const builder = new HmacHeaderBuilder()
        .withAuthorization("someAuthorizationHeaderValue")
        .withPolicy("Some Policy")
        .withDateRequested(new Date())
        .withNonce("Some Nonce");
    
    const headers = builder.build();
    assert.equal(headers["Hmac-Scheme"], null);
    assert.equal(headers["Hmac-Options"], null);
});