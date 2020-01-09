using System;
using System.IO;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexWriter : ITexWriter
    {
        private readonly ITexHeaderWriter _texHeaderWriter;
        private readonly ITexImageContainerWriter _texImageContainerWriter;
        private readonly ITexFrameInfoContainerWriter _texFrameInfoContainerWriter;

        public TexWriter(
            ITexHeaderWriter texHeaderWriter,
            ITexImageContainerWriter texImageContainerWriter,
            ITexFrameInfoContainerWriter texFrameInfoContainerWriter)
        {
            _texHeaderWriter = texHeaderWriter;
            _texImageContainerWriter = texImageContainerWriter;
            _texFrameInfoContainerWriter = texFrameInfoContainerWriter;
        }

        public void WriteTo(BinaryWriter writer, Tex tex)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (tex == null) throw new ArgumentNullException(nameof(tex));
            
            if (tex.Magic1 != "TEXV0005")
                throw new UnknownTexHeaderMagicException(nameof(tex.Magic1), tex.Magic1);

            if (tex.Magic2 != "TEXI0001")
                throw new UnknownTexHeaderMagicException(nameof(tex.Magic2), tex.Magic2);
            
            writer.WriteNString(tex.Magic1);
            writer.WriteNString(tex.Magic2);
            
            _texHeaderWriter.WriteTo(writer, tex.Header);
            _texImageContainerWriter.WriteTo(writer, tex.ImagesContainer);

            _texImageContainerWriter.WriteImagesTo(writer, tex);

            if (tex.IsGif)
                _texFrameInfoContainerWriter.WriteTo(writer, tex.FrameInfoContainer);
        }
    }
}