import { expect, test } from "vitest";
import { HmacPolicyBuilder } from "../../src/Scripts/HmacManager/policies/hmac-policy-builder.js"

test("\"HmacPolicyBuilder\" with 1 policy and 2 schemes.", () => {
    const builder = new HmacPolicyBuilder("Policy")
        .usePublicKey("PublicKey")
        .usePrivateKey("PrivateKey")
        .useContentHashAlgorithm("SHA256")
        .useSigningHashAlgorithm("HMACSHA256")
        .useMemoryCache(1) 
        .addScheme("Scheme_1", scheme => {
            scheme.addHeader("Header-1a");
            scheme.addHeader("Header-1b");
        })
        .addScheme("Scheme_2", scheme => {
            scheme.addHeader("Header-2a");
            scheme.addHeader("Header-2b");
        });

    const policy = builder.build();
    expect(policy.keys.publicKey).toBe("PublicKey");
    expect(policy.keys.privateKey).toBe("PrivateKey");
    expect(policy.algorithms.contentHashAlgorithm).toBe("SHA256");
    expect(policy.algorithms.signingHashAlgorithm).toBe("HMACSHA256");

    const [scheme1, scheme2] = policy.schemes.getAll();
    const [scheme1Headers, scheme2Headers] = [scheme1.getHeaders(), scheme2.getHeaders()];
    expect(scheme1.name).toBe("Scheme_1");
    expect(scheme1Headers[0].name).toBe("Header-1a");
    expect(scheme1Headers[1].name).toBe("Header-1b");
    expect(scheme1Headers[0].claimType).toBe("Header-1a");
    expect(scheme1Headers[1].claimType).toBe("Header-1b");

    expect(scheme2.name).toBe("Scheme_2");
    expect(scheme2Headers[0].name).toBe("Header-2a");
    expect(scheme2Headers[1].name).toBe("Header-2b");
    expect(scheme2Headers[0].claimType).toBe("Header-2a");
    expect(scheme2Headers[1].claimType).toBe("Header-2b");
})