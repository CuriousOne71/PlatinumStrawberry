module PlatBerryPlatinumBlock

using ..Ahorn, Maple

@mapdef Entity "PlatinumStrawberry/PlatinumBlock" PlatinumBlock(x::Integer, y::Integer, width::Integer=Maple.defaultBlockWidth, height::Integer=Maple.defaultBlockHeight)

const placements = Ahorn.PlacementDict(
    "Silver Block (Collab Utils 2)" => Ahorn.EntityPlacement(
        PlatinumBlock,
        "rectangle"
    ),
)

block = "SyrenyxPlatinumStrawberry/objects/platinumblock/block"
icon = "SyrenyxPlatinumStrawberry/objects/platinumblock/icon"

Ahorn.minimumSize(entity::PlatinumBlock) = 16, 16
Ahorn.resizable(entity::PlatinumBlock) = true, true

Ahorn.selection(entity::PlatinumBlock) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::PlatinumBlock, room::Maple.Room)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, block, (i - 1) * 8, 0, 8, 0, 8, 8)
        Ahorn.drawImage(ctx, block, (i - 1) * 8, height - 8, 8, 16, 8, 8)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, block, 0, (i - 1) * 8, 0, 8, 8, 8)
        Ahorn.drawImage(ctx, block, width - 8, (i - 1) * 8, 16, 8, 8, 8)
    end

    for i in 2:tilesWidth - 1, j in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, block, (i - 1) * 8, (j - 1) * 8, 8, 8, 8, 8)
    end

    Ahorn.drawImage(ctx, block, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, block, width - 8, 0, 16, 0, 8, 8)
    Ahorn.drawImage(ctx, block, 0, height - 8, 0, 16, 8, 8)
    Ahorn.drawImage(ctx, block, width - 8, height - 8, 16, 16, 8, 8)

    Ahorn.drawSprite(ctx, icon, width / 2, height / 2)
end

end