using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyboardTextConverter;

/// <summary>
/// Converts text between Thai and English keyboard layouts.
/// Based on QWERTY keyboard layout mapping.
/// </summary>
public class ThaiEnglishConverter
{
    // Thai ↔ English character mapping based on QWERTY layout
    private static readonly Dictionary<char, char> ThaiToEnglish = new()
    {
        // Number row
        ['๏'] = '0',
        ['ๆ'] = '9',
        ['๎'] = '8',
        ['ํ'] = '7',
        ['ุ'] = '6',
        ['ึ'] = '5',
        ['๑'] = '4',
        ['๓'] = '3',
        ['๒'] = '2',
        ['๐'] = '1',

        // Top row (QWERTY)
        ['ๆ'] = 'q',
        ['ไ'] = 'w',
        ['ำ'] = 'e',
        ['พ'] = 'r',
        ['ะ'] = 't',
        ['ั'] = 'y',
        ['า'] = 'u',
        ['สั'] = 'i',
        ['ี'] = 'o',
        ['ึก'] = 'p',

        // Middle row (ASDFGH...)
        ['ั'] = 'a',
        ['หนึ่ง'] = 's',
        ['ิ'] = 'd',
        ['ีั'] = 'f',
        ['ึ'] = 'g',
        ['ั'] = 'h',
        ['า'] = 'j',
        ['ไม่'] = 'k',
        ['ั'] = 'l',

        // Bottom row (ZXCVBN...)
        ['ซ'] = 'z',
        ['ใ'] = 'x',
        ['ฉ'] = 'c',
        ['ว'] = 'v',
        ['บ'] = 'b',
        ['ข'] = 'n',
        ['ค'] = 'm',

        // vowels and tone marks (simplified)
        ['เ'] = 'e',
        ['แ'] = 'a',
        ['โ'] = 'o',
        ['ใ'] = 'i',
        ['ุ'] = 'u',
    };

    private static readonly Dictionary<char, char> EnglishToThai = new();

    static ThaiEnglishConverter()
    {
        // Build reverse mapping
        foreach (var kvp in ThaiToEnglish)
        {
            if (!EnglishToThai.ContainsKey(kvp.Value))
            {
                EnglishToThai[kvp.Value] = kvp.Key;
            }
        }

        InitializeComprehensiveMapping();
    }

    /// <summary>
    /// Initialize comprehensive QWERTY-Thai mapping.
    /// Maps English keyboard characters to Thai equivalent based on QWERTY layout.
    /// </summary>
    private static void InitializeComprehensiveMapping()
    {
        // Common Thai characters mapped to English QWERTY positions
        var comprehensiveMapping = new Dictionary<char, char>
        {
            // Numbers (Shift + number on Thai keyboard produces Thai numerals)
            ['๐'] = '0', // Thai zero
            ['๑'] = '1', // Thai one
            ['๒'] = '2', // Thai two
            ['๓'] = '3', // Thai three
            ['๔'] = '4', // Thai four
            ['๕'] = '5', // Thai five
            ['๖'] = '6', // Thai six
            ['๗'] = '7', // Thai seven
            ['๘'] = '8', // Thai eight
            ['๙'] = '9', // Thai nine

            // Main consonants
            ['ก'] = 'q', // ko kai
            ['ข'] = 'w', // kho khat
            ['ค'] = 'e', // kho khwai
            ['ง'] = 'r', // ngo ngu
            ['จ'] = 't', // cho chang
            ['ฉ'] = 'y', // cho ching
            ['ช'] = 'u', // cho chang
            ['ซ'] = 'i', // so so
            ['ฌ'] = 'o', // cho choe
            ['ญ'] = 'p', // yo ying

            // Second row
            ['ด'] = 'a', // do dek
            ['ต'] = 's', // to tao
            ['ถ'] = 'd', // tho thung
            ['ท'] = 'f', // tho thahan
            ['ธ'] = 'g', // tho thong
            ['น'] = 'h', // no nen
            ['บ'] = 'j', // bo baimai
            ['ป'] = 'k', // po pla
            ['ผ'] = 'l', // pho phung
            ['พ'] = ';', // pho phan

            // Third row
            ['ฟ'] = 'z', // fo fa
            ['ภ'] = 'x', // pho samphao
            ['ม'] = 'c', // mo ma
            ['ย'] = 'v', // yo yak
            ['ร'] = 'b', // ro rue
            ['ล'] = 'n', // lo ling
            ['ว'] = 'm', // wo waen

            // Vowels
            ['ะ'] = 'a', // short a
            ['า'] = 'a', // long a
            ['ิ'] = 'i', // short i
            ['ี'] = 'i', // long i
            ['ึ'] = 'u', // short u
            ['ู'] = 'u', // long u
            ['เ'] = 'e', // short e
            ['แ'] = 'e', // long e
            ['โ'] = 'o', // short o
            ['ๅ'] = 'o', // long o
            ['ใ'] = 'i', // short ia
            ['ๆ'] = 'a', // mai tho

            // Combining marks
            ['่'] = '`', // mai tho (low tone)
            ['้'] = '~', // mai tri (high tone)
            ['๊'] = '^', // mai chattawa (rising tone)
            ['๋'] = '&', // mai chantawa (falling tone)
        };

        // Update mappings
        foreach (var kvp in comprehensiveMapping)
        {
            ThaiToEnglish[kvp.Key] = kvp.Value;
            if (!EnglishToThai.ContainsKey(kvp.Value))
            {
                EnglishToThai[kvp.Value] = kvp.Key;
            }
        }
    }

    /// <summary>
    /// Detect if text contains Thai characters.
    /// </summary>
    public static bool ContainsThai(string text)
    {
        return text.Any(c => IsThai(c));
    }

    /// <summary>
    /// Detect if character is Thai.
    /// Thai Unicode range: U+0E00 to U+0E7F
    /// </summary>
    private static bool IsThai(char c)
    {
        return c >= '\u0E00' && c <= '\u0E7F';
    }

    /// <summary>
    /// Convert text, auto-detecting direction (Thai→English or English→Thai).
    /// </summary>
    public static string Convert(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        if (ContainsThai(text))
            return ConvertThaiToEnglish(text);
        else
            return ConvertEnglishToThai(text);
    }

    /// <summary>
    /// Convert Thai text to English keyboard equivalent.
    /// </summary>
    public static string ConvertThaiToEnglish(string text)
    {
        var result = new System.Text.StringBuilder();

        foreach (char c in text)
        {
            if (ThaiToEnglish.TryGetValue(c, out var englishChar))
            {
                result.Append(englishChar);
            }
            else
            {
                // Keep unmapped characters as-is
                result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Convert English text to Thai keyboard equivalent.
    /// </summary>
    public static string ConvertEnglishToThai(string text)
    {
        var result = new System.Text.StringBuilder();

        foreach (char c in text)
        {
            char lowerC = char.ToLowerInvariant(c);
            if (EnglishToThai.TryGetValue(lowerC, out var thaiChar))
            {
                // Preserve original case if applicable
                if (char.IsUpper(c) && char.IsLetter(lowerC))
                {
                    result.Append(thaiChar); // Thai doesn't have case, just append
                }
                else
                {
                    result.Append(thaiChar);
                }
            }
            else
            {
                // Keep unmapped characters as-is
                result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Get conversion direction from text content.
    /// </summary>
    public static string GetConversionDirection(string text)
    {
        if (ContainsThai(text))
            return "Thai→English";
        else
            return "English→Thai";
    }
}
