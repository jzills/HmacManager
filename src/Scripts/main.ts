import { HmacPolicyBuilder } from "./HmacManager/policies/hmac-policy-builder.js";

var builder = new HmacPolicyBuilder("Policy_1")
    .usePublicKey("MyPublicKey")
    .usePrivateKey("MyPrivateKey")
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
console.dir(policy);

for (const scheme of policy.schemes.getAll()) {
    console.dir(scheme);
}