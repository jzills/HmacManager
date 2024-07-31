import { HmacScheme } from "./hmac-scheme.js"

export type HmacPolicy = {
    name: string,
    publicKey: string,
    privateKey: string,
    schemes: HmacScheme[]
}