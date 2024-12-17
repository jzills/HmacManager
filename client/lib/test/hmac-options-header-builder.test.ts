import { assert, test } from "vitest";
import { HmacAuthenticationDefaults } from "../src/hmac-authentication-defaults";
import HmacOptionsHeaderBuilder from "../src/builders/hmac-options-header-builder";

test("HmacHeaderOptionsBuilder_Build_ExpectOptions", () => {
    const authorization = "someAuthorizationHeaderValue";
    const policy = "Some Policy";
    const scheme = "Some Scheme";
    const dateRequested = new Date();
    const nonce = "Some Nonce";
    const expectedOptions = 
        `${HmacAuthenticationDefaults.Headers.Authorization}=Hmac ${authorization}&` +
        `${HmacAuthenticationDefaults.Headers.Policy}=${policy}&` +
        `${HmacAuthenticationDefaults.Headers.Scheme}=${scheme}&` +
        `${HmacAuthenticationDefaults.Headers.Nonce}=${nonce}&` +
        `${HmacAuthenticationDefaults.Headers.DateRequested}=${dateRequested.getTime()}`;

    const builder = new HmacOptionsHeaderBuilder()
        .withAuthorization(authorization)
        .withPolicy(policy)
        .withScheme(scheme)
        .withDateRequested(dateRequested)
        .withNonce(nonce);
    
    const headers = builder.build();
    assert.equal(headers["Hmac-Options"], btoa(expectedOptions));
    assert.equal(headers.Authorization, `Hmac ${authorization}`);
    assert.equal(headers["Hmac-Policy"], null);
    assert.equal(headers["Hmac-Scheme"], null);
    assert.equal(headers["Hmac-DateRequested"], null);
    assert.equal(headers["Hmac-Nonce"], null);
});

test("HmacHeaderOptionsBuilder_Build_ExpectAllNull", () => {
    const headers = new HmacOptionsHeaderBuilder().build();
    assert.equal(headers["Hmac-Policy"], null);
    assert.equal(headers["Hmac-Scheme"], null);
    assert.equal(headers["Hmac-DateRequested"], null);
    assert.equal(headers["Hmac-Nonce"], null);
});