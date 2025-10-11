from ScriptCollection.TFCPS.Flutter.TFCPS_CodeUnitSpecific_Flutter import TFCPS_CodeUnitSpecific_Flutter_Functions,TFCPS_CodeUnitSpecific_Flutter_CLI


def build():
    tf:TFCPS_CodeUnitSpecific_Flutter_Functions=TFCPS_CodeUnitSpecific_Flutter_CLI.parse(__file__)
    tf.build("con_surv_app",[
            "web",
            "appbundle",
            # "ios",
        ])
 
if __name__ == "__main__":
    build()
