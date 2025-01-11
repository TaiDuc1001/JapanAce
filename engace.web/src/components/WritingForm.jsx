import {
  Box,
  Button,
  FormControl,
  InputLabel,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import PropTypes from "prop-types";

export default function WritingForm({ onClosePannel }) {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [content, setContent] = useState(searchParams.get("content") ?? "");

  const handleSearch = () => {
    if (content) {
      navigate(`?content=${encodeURIComponent(content.trim())}`);
      onClosePannel ? onClosePannel(false) : null;
    }
  };

  return (
    <Box display="flex" flexDirection="column" gap={2} sx={{ width: "100%" }}>
      <Typography variant="h1" textAlign={"center"}>
        LUYỆN VIẾT TIẾNG NHẬT
      </Typography>
      <Box sx={{ width: "100%" }}>
        <Typography
          variant="body1"
          component={InputLabel}
          required
          htmlFor="search"
          sx={{ color: "#202124", marginBottom: "0.5rem" }}
        >
          Nội dung bài viết của bạn
        </Typography>
        <FormControl fullWidth>
          <TextField
            id="search"
            variant="outlined"
            placeholder="例: 私の名前はエースです。ベトナムに住んでいます。小さなソフトウェア会社で働いています。音楽を聴いたり、映画を見たりするのが好きです。日本語はとても難しいです。私は昔から日本語を勉強しています。私の話ははっきりしないので、人に理解してもらうのが難しいです。日本語を話すときは恥ずかしいです。仕事や将来のために日本語を上達させたいです。"
            value={content}
            onChange={(e) => setContent(e.target.value)}
            multiline
            rows={12}
          />
        </FormControl>
      </Box>
      <Button
        variant="contained"
        sx={{
          width: "fit-content",
          alignSelf: "center",
          color: "white",
          "&:not(:disabled)": {
            background: "linear-gradient(45deg, #FE6B8B 30%, #FF8E53 90%)",
            transition:
              "transform 0.3s ease-in-out, box-shadow 0.3s ease-in-out",
            "&:hover": {
              background: "linear-gradient(45deg, #FE6B8B 30%, #FF8E53 90%)",
              opacity: 0.8,
              transform: "scale(1.05)",
              boxShadow: "0 0.2rem 1.2rem rgba(255, 0, 0, 0.2)",
            },
          },
        }}
        size="large"
        disabled={!content.trim()}
        onClick={handleSearch}
      >
        ĐÁNH GIÁ
      </Button>
    </Box>
  );
}

WritingForm.propTypes = {
  onClosePannel: PropTypes.func,
};
