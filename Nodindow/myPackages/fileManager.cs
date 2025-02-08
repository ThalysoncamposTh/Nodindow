using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodindow.myPackages
{
    public static class fileManager
    {
        /// <summary>
        /// Obtém todos os arquivos dentro de uma pasta especificada.
        /// </summary>
        /// <param name="directoryPath">Caminho da pasta.</param>
        /// <param name="searchOption">Opção de busca (TopDirectoryOnly ou AllDirectories).</param>
        /// <returns>Lista de caminhos de arquivos encontrados.</returns>
        public static List<string> GetFilesInDirectory(string directoryPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try
            {
                // Verifica se o diretório existe
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"O diretório '{directoryPath}' não existe.");
                    return new List<string>();
                }

                // Obtém todos os arquivos no diretório
                var files = Directory.GetFiles(directoryPath, "*.*", searchOption);
                return new List<string>(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar o diretório: {ex.Message}");
                return new List<string>();
            }
        }
        /// <summary>
        /// Remove um número especificado de pastas do final do caminho.
        /// </summary>
        /// <param name="path">Caminho completo.</param>
        /// <param name="levelsToRemove">Número de níveis a serem removidos.</param>
        /// <returns>Caminho modificado com os níveis removidos.</returns>
        public static string RemoveLastPathLevels(string path, int levelsToRemove)
        {
            if (levelsToRemove <= 0)
                throw new ArgumentException("O número de níveis a remover deve ser maior que zero.", nameof(levelsToRemove));

            // Normaliza o caminho para evitar problemas com barras
            path = Path.GetFullPath(path);

            // Divide o caminho em partes
            string[] pathParts = path.Split(Path.DirectorySeparatorChar);

            if (levelsToRemove >= pathParts.Length)
                throw new ArgumentException("Os níveis a remover excedem o número de partes no caminho.", nameof(levelsToRemove));

            // Remove os níveis desejados
            string[] remainingParts = new string[pathParts.Length - levelsToRemove];
            Array.Copy(pathParts, remainingParts, remainingParts.Length);

            // Recompõe o caminho
            return string.Join(Path.DirectorySeparatorChar.ToString(), remainingParts);
        }
    }
}
