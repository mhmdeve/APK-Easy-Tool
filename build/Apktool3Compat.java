import java.io.File;
import java.net.URISyntaxException;
import java.util.ArrayList;
import java.util.List;

/**
 * Compatibility launcher for Apktool 3.x.
 *
 * APK Easy Tool 1.60 emits the legacy Apktool 2.x CLI flags. Apktool 3.x
 * intentionally removed several of those flags, so this launcher translates
 * the legacy command line to the Apktool 3.x equivalent before execution.
 */
public final class Apktool3Compat {
    private static final String REAL_JAR = "apktool_3.0.3.bin";

    private Apktool3Compat() {
    }

    public static void main(String[] args) throws Exception {
        File launcherJar = getLauncherJar();
        File realJar = new File(launcherJar.getParentFile(), REAL_JAR);

        if (!realJar.isFile()) {
            System.err.println("Apktool 3.x runtime not found: " + realJar.getAbsolutePath());
            System.exit(2);
        }

        List<String> translated = translate(args);
        List<String> command = new ArrayList<>();
        command.add("java");
        command.add("-jar");
        command.add(realJar.getAbsolutePath());
        command.addAll(translated);

        Process process = new ProcessBuilder(command)
                .inheritIO()
                .start();

        System.exit(process.waitFor());
    }

    private static File getLauncherJar() throws URISyntaxException {
        File location = new File(Apktool3Compat.class
                .getProtectionDomain()
                .getCodeSource()
                .getLocation()
                .toURI());

        if (location.isFile()) {
            return location;
        }

        throw new IllegalStateException("Apktool compatibility launcher is not running from a JAR file.");
    }

    private static List<String> translate(String[] args) {
        List<String> result = new ArrayList<>();
        if (args.length == 0) {
            return result;
        }

        String command = args[0];

        // Apktool 3.x renamed empty-framework-dir to clean-frameworks.
        if ("empty-framework-dir".equals(command) || "efd".equals(command)) {
            result.add("cf");
        } else {
            result.add(command);
        }

        for (int i = 1; i < args.length; i++) {
            String arg = args[i];

            // Decode: legacy short flags -> Apktool 3 long flags.
            if ("d".equals(command) || "decode".equals(command)) {
                if ("-b".equals(arg)) {
                    result.add("--no-debug-info");
                } else if ("-k".equals(arg)) {
                    result.add("--keep-broken-res");
                } else if ("-m".equals(arg)) {
                    result.add("--match-original");
                } else if ("-api".equals(arg) || "--api-level".equals(arg)) {
                    // Apktool 3 automatically detects the API level.
                    if (i + 1 < args.length) {
                        i++;
                    }
                } else if ("--only-main-classes".equals(arg)) {
                    // This is the Apktool 3 default behavior.
                } else if ("--force-manifest".equals(arg)) {
                    // No direct Apktool 3 equivalent.
                } else {
                    result.add(arg);
                }
                continue;
            }

            // Build: legacy short flags -> Apktool 3 long flags.
            if ("b".equals(command) || "build".equals(command)) {
                if ("-c".equals(arg)) {
                    result.add("--copy-original");
                } else if ("-d".equals(arg)) {
                    result.add("--debuggable");
                } else if ("-f".equals(arg)) {
                    result.add("--force");
                } else if ("-nc".equals(arg)) {
                    result.add("--no-crunch");
                } else if ("-api".equals(arg) || "--api-level".equals(arg)) {
                    // Apktool 3 automatically detects the API level.
                    if (i + 1 < args.length) {
                        i++;
                    }
                } else if ("--use-aapt2".equals(arg)) {
                    // Apktool 3 is aapt2-only.
                } else {
                    result.add(arg);
                }
                continue;
            }

            result.add(arg);
        }

        return result;
    }
}
