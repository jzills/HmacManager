import path from "path";
import { defineConfig } from "vite";
import dts from "vite-plugin-dts";

export default defineConfig({
    plugins: [
        dts({
            outDir: "dist",
            rollupTypes: true
        }),
    ],
    build: {
        lib: {
            entry: path.resolve(__dirname, "src/index.ts"),
            name: "HmacManager",
            fileName: "index", 
        },
        rollupOptions: {
            output: [{
                entryFileNames: "index.js",
                format: "esm", 
            },
            {
                entryFileNames: "index.cjs",  
                format: "cjs",
            }],
            external: ["tslib", "node_modules"], 
            treeshake: false
        },
        target: "esnext"
    }
});