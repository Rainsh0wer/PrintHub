-- Convert raw HTML <br> tags into real Word line breaks.
-- Without this, pandoc's docx writer silently drops them and every
-- numbered step inside a use-case table cell runs together on one line.
function RawInline(el)
  if el.format:match('html') and el.text:match('^%s*<br%s*/?>%s*$') then
    return pandoc.LineBreak()
  end
end

-- Same problem for &nbsp; used for indentation in the manual TOC tables.
function Str(el)
  if el.text:find('\194\160') then
    return pandoc.Str(el.text)
  end
end
