import { defineConfig } from 'vitest/config';

/*
 * The angular unit-test-builder does not offer an option for the output-folder of the coverage-report, so the
 * only way to write it to the artifacts-folder of the codeunit is this file. The build-pipeline expects the
 * cobertura-file at "Other/Artifacts/TestCoverage", like for every other codeunit.
 *
 * The provider has to be istanbul and not v8: istanbul writes the cobertura-file with the same library
 * ("istanbul-reports") which karma-coverage used before, so the structure of the file stays the one the
 * build-pipeline reads.
 */
export default defineConfig({
  test: {
    coverage: {
      provider: 'istanbul',
      reportsDirectory: 'Other/Artifacts/TestCoverage',
    },
    // Run all specs in a single reused fork. The Angular+Material module graph is heavy enough that
    // parallel per-file forks ran into vitest's worker-respond timeout on resource-constrained CI runners.
    pool: 'forks',
    poolOptions: {
      forks: {
        singleFork: true,
        maxForks: 1,
        isolate: false,
      },
    },
    testTimeout: 60000,
    hookTimeout: 60000,
    teardownTimeout: 60000,
  },
});
