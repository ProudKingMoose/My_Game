using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static BaseClass;

public class TypeEffectivness : MonoBehaviour
{
    float[][] typeChart =
    {
                                    /* N     H     C     Z     L     D */
        /* NONE */      new float[] { 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F },
        /* HEAT */      new float[] { 1.0F, 0.5F, 1.5F, 0.5F, 0.5F, 1.0F },
        /* CHILL */     new float[] { 1.0F, 1.5F, 0.5F, 0.5F, 1.0F, 1.0F },
        /* ZAPP */      new float[] { 1.0F, 0.5F, 2.0F, 0.5F, 0.5F, 1.0F },
        /* LIGHT */     new float[] { 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 2.0F },
        /* DARKNESS */  new float[] { 1.0F, 1.0F, 1.0F, 1.0F, 2.0F, 1.0F }
    };

    public float GetEffectivness(EnergyType1 typeAttack, EnergyType1 typeDefend , EnergyLevel AttLV)
    {
        if (typeAttack == EnergyType1.None || typeDefend == EnergyType1.None)
            return 1;

        int row = (int)typeAttack;
        int column = (int)typeDefend;
        float LVMult = 0;

        if (AttLV == EnergyLevel.I)
            LVMult = 1;
        else if (AttLV == EnergyLevel.II)
            LVMult = 1.2f;
        else if (AttLV == EnergyLevel.III)
            LVMult = 1.4f;

        float Mult = typeChart[row][column] * LVMult;

        UnityEngine.Debug.Log(typeChart[row][column]);
        UnityEngine.Debug.Log(row);
        UnityEngine.Debug.Log(column);

        return Mult;
    }
}
