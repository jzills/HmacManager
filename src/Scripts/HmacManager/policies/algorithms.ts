export type ContentHashAlgorithm = "SHA1" | "SHA256" | "SHA512";
export type SigningHashAlgorithm = "HMACSHA1" | "HMACSHA256" | "HMACSHA512";
export class Algorithms {
    contentHashAlgorithm: ContentHashAlgorithm;
    signingHashAlgorithm: SigningHashAlgorithm;
}