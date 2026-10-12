from ScriptCollection.TFCPS.Flutter.TFCPS_CodeUnitSpecific_Flutter import TFCPS_CodeUnitSpecific_Flutter_Functions, TFCPS_CodeUnitSpecific_Flutter_CLI


def update_visual_regression_baselines():
    tf: TFCPS_CodeUnitSpecific_Flutter_Functions = TFCPS_CodeUnitSpecific_Flutter_CLI.parse(__file__)
    tf.run_visual_regression_tests_in_container("con_surv_app", True)


if __name__ == "__main__":
    update_visual_regression_baselines()
